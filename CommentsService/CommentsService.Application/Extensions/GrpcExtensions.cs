using Microsoft.Extensions.DependencyInjection;
using OtusForum.AuthUsersService.Grpc;

namespace CommentsService.Application.Extensions;

public static class GrpcExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        services.AddGrpcClient<UsersGrpcApi.UsersGrpcApiClient>(options =>
        {
            options.Address = new System.Uri("http://auth-users-service:5226");
        });

        return services;
    }
}
