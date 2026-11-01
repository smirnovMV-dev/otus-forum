using AuthUsersService.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AuthUsersService.Infrastructure.Data;

internal sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        UserEntityToTableMapper.Map(modelBuilder);
        RoleEntityToTableMapper.Map(modelBuilder);
        UserRoleEntityToTableMapper.Map(modelBuilder);
    }
}
