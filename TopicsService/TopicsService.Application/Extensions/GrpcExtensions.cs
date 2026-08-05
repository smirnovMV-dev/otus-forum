using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OtusForum.AuthUsersService.Grpc;

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
            options.Address = new System.Uri("https://localhost:7195");
        });

        return services;
    }
}
