using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Repositories.Users.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Infrastructure.Repositories.Users;

public interface IUserRepository
{
    Task<int> CreateAsync(
        User user,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<UserNicknameModel>> GetNicknamesAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken);
}
