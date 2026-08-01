using AuthUsersService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthUsersService.Infrastructure.Mappers;

internal static class RoleEntityToTableMapper
{
    public static ModelBuilder Map(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles", "public");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();

            entity.Property(r => r.Name)
                .HasColumnName("name")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(r => r.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasIndex(r => r.Name)
                .IsUnique();
        });

        return modelBuilder;
    }
}
