using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthUsersService.Domain;
using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthUsersService.Infrastructure.Repositories;

internal sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task RevokeAsync(long refreshTokenId, string? reason = null, CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens.FindAsync([refreshTokenId], cancellationToken);
        if (token is not null && token.RevokedAt == null)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedBy = reason;
            _context.RefreshTokens.Update(token);
        }
    }

    public async Task RevokeAllForUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedBy = "revoked_on_login";
        }

        if (tokens.Any())
        {
            _context.RefreshTokens.UpdateRange(tokens);
        }
    }

    public async Task DeleteExpiredAsync(DateTimeOffset before, CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt < before && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        _context.RefreshTokens.RemoveRange(expiredTokens);
    }
}
