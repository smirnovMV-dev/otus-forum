using AuthUsersService.Domain;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Roles.GetAllRoles;

public sealed record GetAllRolesCommand() : IRequest<IReadOnlyCollection<RoleDto>>;

public sealed record RoleDto
{
    public long Id { get; init; }
    public RoleName Name { get; init; }
}

internal sealed class GetAllRolesCommandHandler : IRequestHandler<GetAllRolesCommand, IReadOnlyCollection<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetAllRolesCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IReadOnlyCollection<RoleDto>> Handle(
        GetAllRolesCommand request,
        CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);

        return roles
            .OrderBy(r => r.Id)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToList();
    }
}
