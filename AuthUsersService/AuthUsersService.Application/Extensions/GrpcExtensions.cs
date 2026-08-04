using AuthUsersService.Application.Services.External.UsersGrpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AuthUsersService.Application.Extensions;

public static class GrpcExtensions
{
    public static IEndpointRouteBuilder MapInternalGrpcServices(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<UsersGrpcService>();
        return endpoints;
    }
}
