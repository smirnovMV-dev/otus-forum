using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;

namespace AuthUsersService.Infrastructure.Mappers;

internal static class UserRoleExtensions
{
    public static UserRoleEntity ToEntity(this UserRole domain)
    {
        return new UserRoleEntity
        {
            UserId = domain.UserId,
            RoleId = domain.RoleId,
            CreatedAt = domain.CreatedAt,
            ExpiresAt = domain.ExpiresAt
        };
    }

    public static UserRole ToDomain(this UserRoleEntity entity)
    {
        return UserRole.Create(
            entity.UserId,
            entity.RoleId,
            entity.CreatedAt,
            entity.ExpiresAt);
    }
}
