using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Repositories.Roles;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Roles.CreateRole;

public sealed record CreateRoleCommand(
    string RoleName) : IRequest<int>;


internal sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, int>
{
    private readonly IRoleRepository _roleRepository;

    public CreateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<int> Handle(
        CreateRoleCommand command,
        CancellationToken cancellationToken)
    {
        var role = Role.Create(
            command.RoleName,
            DateTimeOffset.UtcNow);

        return await _roleRepository.CreateAsync(
            role,
            cancellationToken);
    }
}
