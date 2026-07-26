using AuthUsersService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Infrastructure.Repositories.Users;

public interface IUserRepository
{
    Task<int> CreateAsync(
        User user,
        CancellationToken cancellationToken);
}
