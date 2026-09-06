using OtusForum.UI.Clients.Comments;
using OtusForum.UI.Clients.Topics;
using OtusForum.UI.Clients.Users;
using UIService.Clients;
using UIService.Components;
using UIService.Handlers;
using UIService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Регистрируем автосгенерированный клиент
builder.Services.AddTransient<ErrorLoggingHandler>();

// Named HttpClients for general use.
builder.Services.AddHttpClient("TopicsClient", client => { client.BaseAddress = new Uri("http://topics-service:5294/"); });
builder.Services.AddHttpClient("UsersClient", client => { client.BaseAddress = new Uri("http://auth-users-service:5225/"); });
builder.Services.AddHttpClient("CommentsClient", client => { client.BaseAddress = new Uri("http://comments-service:5044/"); });

// API client registrations using factory delegates (avoid hardcoded BaseUrl from generated clients).
builder.Services.AddTransient<ITopicsClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var config = sp.GetRequiredService<IConfiguration>();
    var url = config.GetValue<string>("ApiUrls:Topics") ?? "http://topics-service:5294";
    var client = factory.CreateClient();
    client.BaseAddress = new Uri(url);
    return new TopicsClient(client) { BaseUrl = url.TrimEnd('/') };
});

builder.Services.AddTransient<IUsersClient, UsersClientWrapper>();

builder.Services.AddTransient<ICommentsClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var config = sp.GetRequiredService<IConfiguration>();
    var url = config.GetValue<string>("ApiUrls:Comments") ?? "http://comments-service:5044";
    var client = factory.CreateClient();
    client.BaseAddress = new Uri(url);
    return new CommentsClient(client) { BaseUrl = url.TrimEnd('/') };
});


// Register authentication service
builder.Services.AddScoped<AuthService>();

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