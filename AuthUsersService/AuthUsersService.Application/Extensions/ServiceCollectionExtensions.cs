using AuthUsersService.Application.Auth;
using AuthUsersService.Application.Auth.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthUsersService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        services.AddFluentValidationAutoValidation();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
