using AuthUsersService.Domain.Entities;
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
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        UserEntityToTableMapper.Map(modelBuilder);
        RoleEntityToTableMapper.Map(modelBuilder);
        UserRoleEntityToTableMapper.Map(modelBuilder);

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TokenHash).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(256);
            entity.Property(e => e.UserIdRef).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
