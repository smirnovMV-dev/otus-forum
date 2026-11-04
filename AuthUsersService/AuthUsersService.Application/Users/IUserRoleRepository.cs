using AuthUsersService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Users;

public interface IUserRoleRepository
{
    Task<int> CreateAsync(
        UserRole userRole,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        long userId,
        long roleId,
        CancellationToken cancellationToken);
}
