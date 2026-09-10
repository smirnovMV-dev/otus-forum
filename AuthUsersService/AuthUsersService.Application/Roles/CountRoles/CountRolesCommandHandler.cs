using AuthUsersService.Infrastructure.Repositories.Roles;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Roles.CountRoles;

public sealed record CountRolesCommand() : IRequest<int>;

internal sealed class CountRolesCommandHandler : IRequestHandler<CountRolesCommand, int>
{
    private readonly IRoleRepository _roleRepository;

    public CountRolesCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<int> Handle(
        CountRolesCommand request,
        CancellationToken cancellationToken)
    {
        return await _roleRepository.CountAsync(cancellationToken);
    }
}
