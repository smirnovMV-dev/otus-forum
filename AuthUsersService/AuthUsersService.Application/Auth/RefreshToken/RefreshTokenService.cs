using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuthUsersService.Application.Auth.Models;
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
        string refreshTokenCookie,
        long userId,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeHash(refreshTokenCookie);

        var storedToken = await _refreshTokenRepository
            .FindByTokenHashAsync(tokenHash, cancellationToken);

        // Check: not expired, not revoked, belongs to user
        if (storedToken is null 
            || storedToken.ExpiresAt <= DateTime.UtcNow 
            || storedToken.RevokedAt.HasValue 
            || storedToken.UserId != userId)
        {
            return RefreshResult.Failure("Invalid or expired refresh token.");
        }

        // Get user roles
        var rolesList = await _userRepository.GetUserRoleNamesAsync(userId, cancellationToken);

        // Revoke old refresh token
        await _refreshTokenRepository.RevokeAsync(storedToken.Id, "refreshed", cancellationToken);

        // Generate new access token (JWT)
        var newAccessToken = _jwtTokenGenerator.GenerateTokenFromUserId(userId, rolesList);

        // Generate new refresh token
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshTokenHash = ComputeHash(newRefreshToken);

        // Save new refresh token
        var newStoredToken = new RT
        {
            UserId = userId,
            TokenHash = newRefreshTokenHash,
            UserIdRef = userId.ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(newStoredToken, cancellationToken);

        return RefreshResult.Success(newAccessToken);
    }

    public async Task<bool> RevokeAsync(
        string refreshTokenCookie,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeHash(refreshTokenCookie);

        var storedToken = await _refreshTokenRepository
            .FindByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is not null && storedToken.RevokedAt == null)
        {
            await _refreshTokenRepository.RevokeAsync(storedToken.Id, "logout", cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<bool> IsSessionValidAsync(
        string refreshTokenCookie,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeHash(refreshTokenCookie);

        var storedToken = await _refreshTokenRepository
            .FindByTokenHashAsync(tokenHash, cancellationToken);

        return storedToken is not null 
            && storedToken.ExpiresAt > DateTime.UtcNow 
            && storedToken.RevokedAt == null;
    }

    private static string ComputeHash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

public sealed record RefreshResult(string? Token, string? Error)
{
    public bool IsSuccess => Error is null;
    public static RefreshResult Success(string token) => new(token, null);
    public static RefreshResult Failure(string error) => new(null, error);
}
