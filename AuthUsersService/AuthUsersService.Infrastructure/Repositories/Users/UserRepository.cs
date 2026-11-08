using AuthUsersService.Application.Users;
using AuthUsersService.Domain;
using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;
using AuthUsersService.Infrastructure.Mappers;
using AuthUsersService.Infrastructure.Repositories.Users.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Infrastructure.Repositories.Users;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        ApplicationDbContext context,
        ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CreateAsync(
        User user,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.Users.AddAsync(user.ToEntity());
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

    public async Task<IReadOnlyCollection<UserNicknameModel>> GetNicknamesAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken)
    {
        if (userIds == null || userIds.Count == 0)
        {
            return [];
        }

        try
        {
            var results = await _context.Users
                .AsNoTracking()
                .Where(user => userIds.Contains(user.Id))
                .Select(user => new UserNicknameDto(user.Id, user.Nickname))
                .ToListAsync(cancellationToken);

            return [.. results.Select(dto => new UserNicknameModel(dto.UserId, dto.Nickname))];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return [];
        }
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<int> CountAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RoleName>> GetUserRoleNamesAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(
                _context.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => role.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<long>> GetUserRoleIdsAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task UpdatePasswordAsync(
        long userId,
        string newPasswordHash,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        user.PasswordHash = newPasswordHash;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserSearchModel>> SearchByNicknameAsync(
        string nicknamePart,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(nicknamePart))
            return [];

        try
        {
            var results = await _context.Users
                .AsNoTracking()
                .Where(u => u.Nickname.Contains(nicknamePart))
                .Select(u => new UserSearchModel(u.Id, u.Nickname, u.Email))
                .ToListAsync(cancellationToken);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users by nickname: {NicknamePart}", nicknamePart);
            return [];
        }
    }
}
