using Microsoft.Extensions.DependencyInjection;
using TopicsService.Application.Services.Externals.UsersApiService;

namespace TopicsService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddTransient<IUsersApiServiceAdapter, UsersApiServiceAdapter>();
    }
}
