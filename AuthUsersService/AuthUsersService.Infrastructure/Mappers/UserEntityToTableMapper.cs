using AuthUsersService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthUsersService.Infrastructure.Mappers;

internal static class UserEntityToTableMapper
{
    public static ModelBuilder Map(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "public");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();

            entity.Property(u => u.Nickname)
                .HasColumnName("nickname")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(u => u.Email)
                .HasColumnName("email")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasIndex(u => u.Nickname).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        return modelBuilder;
    }
}
