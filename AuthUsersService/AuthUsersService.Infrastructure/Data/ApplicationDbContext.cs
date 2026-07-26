using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AuthUsersService.Infrastructure.Data;

internal sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        UserEntityToTableMapper.Map(modelBuilder);
        RoleEntityToTableMapper.Map(modelBuilder);
        UserRoleEntityToTableMapper.Map(modelBuilder);
    }
}
