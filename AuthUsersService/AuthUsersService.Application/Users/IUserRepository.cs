using AuthUsersService.Domain;
using AuthUsersService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Users;

public interface IUserRepository
{
    Task<int> CreateAsync(
        User user,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<UserNicknameModel>> GetNicknamesAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken);

    Task<User?> GetByNicknameAsync(
        string nickname,
        CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task<int> CountAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RoleName>> GetUserRoleNamesAsync(
        long userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<long>> GetUserRoleIdsAsync(
        long userId,
        CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(
        long userId,
        CancellationToken cancellationToken);

    Task UpdatePasswordAsync(
        long userId,
        string newPasswordHash,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<UserSearchModel>> SearchByNicknameAsync(
        string nicknamePart,
        CancellationToken cancellationToken);
}
