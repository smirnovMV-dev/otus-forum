using System;
using System.Threading;
using System.Threading.Tasks;
using AuthUsersService.Domain.Entities;

namespace AuthUsersService.Domain;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(long refreshTokenId, string? reason = null, CancellationToken cancellationToken = default);
    Task RevokeAllForUserAsync(long userId, CancellationToken cancellationToken = default);
    Task DeleteExpiredAsync(DateTimeOffset before, CancellationToken cancellationToken = default);
}
