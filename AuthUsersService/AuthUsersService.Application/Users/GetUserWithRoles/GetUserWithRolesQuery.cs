using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AuthUsersService.Application.Users;
using AuthUsersService.Domain;
using MediatR;

namespace AuthUsersService.Application.Users.GetUserWithRoles;

public sealed record GetUserWithRolesQuery(long UserId) : IRequest<UserWithRolesModel>;

internal sealed class GetUserWithRolesQueryHandler : IRequestHandler<GetUserWithRolesQuery, UserWithRolesModel>
{
    private readonly IUserRepository _userRepository;

    public GetUserWithRolesQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserWithRolesModel> Handle(
        GetUserWithRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        var roles = await _userRepository.GetUserRoleNamesAsync(request.UserId, cancellationToken);
        return new UserWithRolesModel(user.Id, user.Nickname, user.Email, roles);
    }
}
