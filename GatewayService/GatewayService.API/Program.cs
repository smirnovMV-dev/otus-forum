var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUIService", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetValue<string[]>("Cors:AllowedOrigins") ?? new[] { "https://localhost:7126" })
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowUIService");
app.MapReverseProxy();

app.Run();
