using AuthUsersService.Application.Users;
using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;
using AuthUsersService.Infrastructure.Mappers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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
}
