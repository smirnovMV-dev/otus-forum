using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Infrastructure.Repositories.Roles;

internal sealed class RoleRepository : IRoleRepository
{
    private readonly ILogger<RoleRepository> _logger;
    private readonly ApplicationDbContext _context;

    public RoleRepository(
        ApplicationDbContext context,
        ILogger<RoleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CreateAsync(
        Role role,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.Roles.AddAsync(role);
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }        
    }
}
