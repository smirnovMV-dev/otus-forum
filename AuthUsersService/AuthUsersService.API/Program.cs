using AuthUsersService.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories();

var app = builder.Build();

app.Run();
