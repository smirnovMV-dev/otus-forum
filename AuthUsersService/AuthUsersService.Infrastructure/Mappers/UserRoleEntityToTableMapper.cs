using AuthUsersService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthUsersService.Infrastructure.Mappers;

internal static class UserRoleEntityToTableMapper
{
    public static ModelBuilder Map(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(static b =>
        {
            b.ToTable("user_roles", "public");

            b.HasKey(ur => new { ur.UserId, ur.RoleId });

            b.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            b.Property(x => x.RoleId)
                .HasColumnName("role_id")
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            b.Property(x => x.ExpiresAt)
                .HasColumnName("expires_at")
                .HasColumnType("timestamp with time zone");
        });

        return modelBuilder;
    }
}
