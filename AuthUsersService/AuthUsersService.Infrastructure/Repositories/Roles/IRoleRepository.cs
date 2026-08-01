using AuthUsersService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Infrastructure.Repositories.Roles;

public interface IRoleRepository
{
    Task<int> CreateAsync(
        Role role,
        CancellationToken cancellationToken);
}
