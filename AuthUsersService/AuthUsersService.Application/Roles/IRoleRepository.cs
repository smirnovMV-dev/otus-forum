using AuthUsersService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Roles;

public interface IRoleRepository
{
    Task<int> CreateAsync(
        Role role,
        CancellationToken cancellationToken);

    Task<int> CountAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Role>> GetAllAsync(
        CancellationToken cancellationToken);
}
