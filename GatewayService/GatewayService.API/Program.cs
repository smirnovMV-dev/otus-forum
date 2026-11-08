using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSettings["SigningKey"] ?? throw new InvalidOperationException("JWT SigningKey not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUIService", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetValue<string[]>("Cors:AllowedOrigins") ?? new[] { "https://localhost:7126" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Logging.AddSeq(builder.Configuration.GetSection("Logging:Seq"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowUIService");
app.UseAuthentication();
app.UseAuthorization();

// Middleware для извлечения User-Id из JWT и прокидывания в заголовке.
// Входящие X-* заголовки клиента не доверяем: всегда удаляем и заполняем только из claims.
app.Use(async (context, next) =>
{
    context.Request.Headers.Remove("X-UserId");
    context.Request.Headers.Remove("X-IsAdmin");
    context.Request.Headers.Remove("X-IsModerator");

    if (context.User.Identity?.IsAuthenticated == true)
    {
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
            ?? context.User.FindFirst("sub");

        if (userIdClaim != null)
        {
            context.Request.Headers["X-UserId"] = userIdClaim.Value;
        }

        var isAdminClaim = context.User.FindFirst("IsAdmin");
        if (isAdminClaim != null)
        {
            context.Request.Headers["X-IsAdmin"] = isAdminClaim.Value;
        }

        var isModeratorClaim = context.User.FindFirst("IsModerator");
        if (isModeratorClaim != null)
        {
            context.Request.Headers["X-IsModerator"] = isModeratorClaim.Value;
        }
    }

    await next.Invoke();
});

app.MapReverseProxy();

app.Run();
