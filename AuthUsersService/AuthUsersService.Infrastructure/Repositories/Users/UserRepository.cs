using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;
using AuthUsersService.Infrastructure.Repositories.Users.Dtos;
using AuthUsersService.Infrastructure.Repositories.Users.Models;
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
            await _context.Users.AddAsync(user);
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

            return [.. results.Select(UserNicknameModel.Create)];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return [];
        }
    }
}
