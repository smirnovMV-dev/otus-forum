using AuthUsersService.Application.Users.GetNicknames;
using Grpc.Core;
using MediatR;
using OtusForum.AuthUsersService.Grpc;
using System.Linq;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Services.External.UsersGrpc;

public sealed class UsersGrpcService : UsersGrpcApi.UsersGrpcApiBase
{
    private readonly IMediator _mediator;

    public UsersGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetNicknamesResponse> GetNicknames(
        GetNicknamesRequest request, 
        ServerCallContext context)
    {
        var command = new GetNicknamesCommand(request.UserIds);

        var results = await _mediator.Send(command, cancellationToken: context.CancellationToken);

        return new GetNicknamesResponse
        {
            UsersNicknames =
            {
                results.Select(r => new GetNicknamesResponse.Types.UserNickname
                {
                    UserId = r.UserId,
                    Nickname = r.Nickname,
                })
            }
        };
    }
}
