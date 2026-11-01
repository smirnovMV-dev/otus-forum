using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;

namespace AuthUsersService.Infrastructure.Mappers;

internal static class RoleExtensions
{
    public static RoleEntity ToEntity(this Role domain)
    {
        return new RoleEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            CreatedAt = domain.CreatedAt
        };
    }

    public static Role ToDomain(this RoleEntity entity)
    {
        var domain = Role.Create(entity.Name, entity.CreatedAt);
        domain.SetId(entity.Id);
        return domain;
    }
}
