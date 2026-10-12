using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopicsService.Application.Services;
using TopicsService.Application.Services.Externals.CommentsApiService;
using TopicsService.Application.Services.Externals.UsersApiService;

namespace TopicsService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddTransient<IUsersApiServiceAdapter, UsersApiServiceAdapter>();
        services.AddTransient<ICommentsApiServiceAdapter, CommentsApiServiceAdapter>();

        services.Configure<TopicCleanerSettings>(
            configuration.GetSection("TopicCleaner"));
        services.AddHostedService<TopicCleanerBackgroundService>();
    }
}
