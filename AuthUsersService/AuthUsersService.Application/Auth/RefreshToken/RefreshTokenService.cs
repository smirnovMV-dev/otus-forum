using System;
using System.Threading;
using System.Threading.Tasks;
using AuthUsersService.Application.Auth.Models;
using AuthUsersService.Application.Auth.Tokens;
using AuthUsersService.Application.Users;
using AuthUsersService.Domain;
using RT = AuthUsersService.Domain.Entities.RefreshToken;

namespace AuthUsersService.Application.Auth.RefreshToken;

public sealed class RefreshTokenService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;

    public RefreshTokenService(
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
    }

    public async Task<RefreshResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return RefreshResult.Failure("Invalid or expired refresh token.");
        }

        var tokenHash = RefreshTokenHasher.ComputeHash(refreshToken);

        var storedToken = await _refreshTokenRepository
            .FindByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || storedToken.IsExpired)
        {
            return RefreshResult.Failure("Invalid or expired refresh token.");
        }

        if (storedToken.IsRevoked)
        {
            // Повторное использование уже ротированного токена трактуем как кражу:
            // отзываем все живые сессии пользователя.
            if (string.Equals(storedToken.RevokedBy, RefreshTokenRevokeReason.Refreshed, StringComparison.Ordinal))
            {
                await _refreshTokenRepository.RevokeAllForUserAsync(
                    storedToken.UserId,
                    RefreshTokenRevokeReason.ReuseDetected,
                    cancellationToken);

                return RefreshResult.Failure("Refresh token reuse detected. All sessions have been revoked.");
            }

            return RefreshResult.Failure("Refresh token has been revoked.");
        }

        var userId = storedToken.UserId;

        var rolesList = await _userRepository.GetUserRoleNamesAsync(userId, cancellationToken);

        var newAccessToken = _jwtTokenGenerator.GenerateTokenFromUserId(userId, rolesList);

        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var newStoredToken = new RT
        {
            UserId = userId,
            TokenHash = RefreshTokenHasher.ComputeHash(newRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.RotateAsync(
            storedToken.Id,
            RefreshTokenRevokeReason.Refreshed,
            newStoredToken,
            cancellationToken);

        return RefreshResult.Success(newAccessToken, newRefreshToken);
    }

    public async Task<long?> GetSessionUserIdAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await FindActiveTokenAsync(refreshToken, cancellationToken);

        return storedToken?.UserId;
    }

    public async Task<bool> RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = RefreshTokenHasher.ComputeHash(refreshToken);

        var storedToken = await _refreshTokenRepository
            .FindByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is not null && storedToken.RevokedAt == null)
        {
            await _refreshTokenRepository.RevokeAsync(
                storedToken.Id,
                RefreshTokenRevokeReason.Logout,
                cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<bool> IsSessionValidAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = RefreshTokenHasher.ComputeHash(refreshToken);

        var storedToken = await _refreshTokenRepository
            .FindByTokenHashAsync(tokenHash, cancellationToken);

        return storedToken is not null
            && storedToken.IsActive;
    }

    private async Task<RT?> FindActiveTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var tokenHash = RefreshTokenHasher.ComputeHash(refreshToken);

        var storedToken = await _refreshTokenRepository
            .FindByTokenHashAsync(tokenHash, cancellationToken);

        return storedToken is not null && storedToken.IsActive ? storedToken : null;
    }
}

public sealed record RefreshResult(string? Token, string? RefreshToken, string? Error)
{
    public bool IsSuccess => Error is null;
    public static RefreshResult Success(string token, string refreshToken) => new(token, refreshToken, null);
    public static RefreshResult Failure(string error) => new(null, null, error);
}