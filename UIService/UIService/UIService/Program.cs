using Microsoft.AspNetCore.DataProtection;
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

// Register AuthHeaderHandler for JWT token injection
builder.Services.AddTransient<AuthHeaderHandler>();

// Register error logging handler
builder.Services.AddTransient<ErrorLoggingHandler>();

// Register NSwag API clients through GatewayService
var gatewayUrl = builder.Configuration.GetValue<string>("GatewayUrl") ?? "http://gateway-service:5000";

builder.Services.AddHttpClient("TopicsClient", client =>
{
    client.BaseAddress = new Uri($"{gatewayUrl}/api/topics/");
    client.Timeout = TimeSpan.FromMinutes(1);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddHttpClient("UsersClient", client =>
{
    client.BaseAddress = new Uri($"{gatewayUrl}/api/auth/");
    client.Timeout = TimeSpan.FromMinutes(1);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddHttpClient("CommentsClient", client =>
{
    client.BaseAddress = new Uri($"{gatewayUrl}/api/comments/");
    client.Timeout = TimeSpan.FromMinutes(1);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// Register NSwag clients with explicit BaseUrl (generated clients have hardcoded localhost in constructor)
builder.Services.AddTransient<ITopicsClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("TopicsClient");
    return new TopicsClient(client) { BaseUrl = client.BaseAddress.ToString().TrimEnd('/') };
});

builder.Services.AddTransient<IUsersClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("UsersClient");
    return new UsersClient(client) { BaseUrl = client.BaseAddress.ToString().TrimEnd('/') };
});

builder.Services.AddTransient<ICommentsClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("CommentsClient");
    return new CommentsClient(client) { BaseUrl = client.BaseAddress.ToString().TrimEnd('/') };
});

// Register authentication and refresh services
builder.Services.AddScoped<CommentRefreshService>();

// Register AdminService using IUsersClient from DI
builder.Services.AddScoped<IAdminService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var authService = sp.GetRequiredService<AuthService>();
    var usersClient = sp.GetRequiredService<IUsersClient>();
    return new AdminService(httpClientFactory, authService, usersClient);
});

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