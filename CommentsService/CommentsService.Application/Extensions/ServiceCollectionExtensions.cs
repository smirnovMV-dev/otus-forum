using CommentsService.Application.Services.Externals.UsersApiService;
using Microsoft.Extensions.DependencyInjection;
using OtusForum.AuthUsersService.Grpc;

namespace CommentsService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddTransient<IUsersApiServiceAdapter, UsersApiServiceAdapter>();

        services.AddGrpcClient<UsersGrpcApi.UsersGrpcApiClient>(options =>
        {
            options.Address = new System.Uri("http://auth-users-service:5226");
        });
    }
}
