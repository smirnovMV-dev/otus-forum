using System.Threading;
using System.Threading.Tasks;

using AuthUsersService.Application.Users;
using MediatR;

namespace AuthUsersService.Application.Users.RemoveUserRole;

public sealed record RemoveUserRoleCommand(long UserId, long RoleId) : IRequest<RemoveUserRoleResponse>;

public sealed record RemoveUserRoleResponse();

internal sealed class RemoveUserRoleCommandHandler : IRequestHandler<RemoveUserRoleCommand, RemoveUserRoleResponse>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public RemoveUserRoleCommandHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<RemoveUserRoleResponse> Handle(
        RemoveUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _userRoleRepository.RemoveAsync(request.UserId, request.RoleId, cancellationToken);
        return new RemoveUserRoleResponse();
    }
}
