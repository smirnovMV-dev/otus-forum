using AuthUsersService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Infrastructure.Repositories.UserRoles;

public interface IUserRoleRepository
{
    Task<int> CreateAsync(
        UserRole userRole,
        CancellationToken cancelationtoken);
}
