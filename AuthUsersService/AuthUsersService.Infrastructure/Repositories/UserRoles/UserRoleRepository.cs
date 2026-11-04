using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AuthUsersService.Application.Users;
using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;
using AuthUsersService.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthUsersService.Infrastructure.Repositories.UserRoles;

internal sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly ILogger<UserRoleRepository> _logger;
    private readonly ApplicationDbContext _context;

    public UserRoleRepository(
        ApplicationDbContext context,
        ILogger<UserRoleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CreateAsync(
        UserRole userRole,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.UserRoles.AddAsync(userRole.ToEntity());
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

    public async Task RemoveAsync(
        long userId,
        long roleId,
        CancellationToken cancellationToken)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (userRole == null)
            throw new KeyNotFoundException($"UserRole for UserId={userId} and RoleId={roleId} not found.");

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
