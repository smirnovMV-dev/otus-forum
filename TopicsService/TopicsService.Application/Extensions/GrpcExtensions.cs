using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OtusForum.AuthUsersService.Grpc;
using OtusForum.CommentsService.Grpc;

namespace TopicsService.Application.Extensions;

public static class GrpcExtensions
{
    public static IEndpointRouteBuilder MapInternalGrpcServices(
        this IEndpointRouteBuilder endpoints)
    {
        
        return endpoints;
    }

    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        services.AddGrpcClient<UsersGrpcApi.UsersGrpcApiClient> (options =>
        {
            options.Address = new System.Uri("http://auth-users-service:5226");
        });

        services.AddGrpcClient<CommentsGrpcApi.CommentsGrpcApiClient>(options =>
        {
            options.Address = new System.Uri("http://comments-service:5045");
        });

        return services;
    }
}
