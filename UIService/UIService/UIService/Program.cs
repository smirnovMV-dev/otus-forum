using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OtusForum.UI.Clients.Comments;
using OtusForum.UI.Clients.Topics;
using OtusForum.UI.Clients.Users;
using UIService.Components;
using UIService.Handlers;
using UIService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure DataProtection to persist keys across container restarts
var keysPath = "/shared-keys";
Directory.CreateDirectory(keysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Register authentication service
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProtectedLocalStorage>();

// Register AuthHeaderHandler for JWT token injection.
// Transient обязателен: каждому HttpClient нужен собственный экземпляр обработчика.
builder.Services.AddTransient<AuthHeaderHandler>();

// Register error logging handler
builder.Services.AddTransient<ErrorLoggingHandler>();

// Register NSwag API clients through GatewayService
var gatewayUrl = builder.Configuration.GetValue<string>("GatewayUrl") ?? "http://gateway-service:5000";

// Named client for refresh/revoke/me calls (registered without AuthHeaderHandler to avoid refresh loops)
builder.Services.AddHttpClient("AuthRefreshClient", client =>
{
    client.BaseAddress = new Uri($"{gatewayUrl}/api/auth/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// NSwag-клиенты регистрируются как scoped (scope цепи Blazor), а HttpClient с AuthHeaderHandler
// создаётся в этом же scope. Через AddHttpMessageHandler так нельзя: IHttpClientFactory разрешает
// обработчики в собственном scope, где AuthService пуст, и JWT не попадал в заголовок Authorization.
builder.Services.AddScoped<ITopicsClient>(sp =>
{
    var httpClient = CreateApiHttpClient(sp, $"{gatewayUrl}/api/topics/");
    return new TopicsClient(httpClient) { BaseUrl = httpClient.BaseAddress!.ToString().TrimEnd('/') };
});

builder.Services.AddScoped<IUsersClient>(sp =>
{
    var httpClient = CreateApiHttpClient(sp, $"{gatewayUrl}/api/auth/");
    return new UsersClient(httpClient) { BaseUrl = httpClient.BaseAddress!.ToString().TrimEnd('/') };
});

builder.Services.AddScoped<ICommentsClient>(sp =>
{
    var httpClient = CreateApiHttpClient(sp, $"{gatewayUrl}/api/comments/");
    return new CommentsClient(httpClient) { BaseUrl = httpClient.BaseAddress!.ToString().TrimEnd('/') };
});

// Register authentication and refresh services
builder.Services.AddScoped<CommentRefreshService>();

builder.Logging.AddSeq(builder.Configuration.GetSection("Logging:Seq"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(UIService.Client._Imports).Assembly);

app.Run();

// Создаёт HttpClient с AuthHeaderHandler, разрешённым в scope вызывающего кода (цепи Blazor),
// чтобы обработчик видел AuthService текущей сессии и подставлял актуальный access-токен.
static HttpClient CreateApiHttpClient(IServiceProvider services, string baseAddress)
{
    var authHeaderHandler = services.GetRequiredService<AuthHeaderHandler>();

    authHeaderHandler.InnerHandler = new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
    };

    return new HttpClient(authHeaderHandler)
    {
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromMinutes(1)
    };
}