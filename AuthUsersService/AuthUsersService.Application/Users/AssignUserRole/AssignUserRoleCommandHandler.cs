using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Repositories.UserRoles;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Users.AssignUserRole;

public sealed record AssignUserRoleCommand(
    long UserId,
    long RoleId,
    DateTimeOffset? ExpiresAt) : IRequest<int>;

internal class AssignUserRoleCommandHandler : IRequestHandler<AssignUserRoleCommand, int>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public AssignUserRoleCommandHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<int> Handle(
        AssignUserRoleCommand command,
        CancellationToken cancellationToken)
    {
        var userRole = UserRole.Create(
            command.UserId,
            command.RoleId,
            DateTimeOffset.UtcNow,
            command.ExpiresAt);
        
        return await _userRoleRepository.CreateAsync(
            userRole,
            cancellationToken);
    }
}
