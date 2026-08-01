using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Infrastructure.Repositories.UserRoles;

internal sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRoleRepository> _logger;

    public UserRoleRepository(
        ApplicationDbContext context,
        ILogger<UserRoleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CreateAsync(
        UserRole userRole,
        CancellationToken cancelationtoken)
    {
        try
        {
            await _context.UserRoles.AddAsync(userRole);
            return await _context.SaveChangesAsync(cancelationtoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
