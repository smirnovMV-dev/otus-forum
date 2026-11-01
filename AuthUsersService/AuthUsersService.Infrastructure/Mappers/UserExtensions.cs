using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Data;

namespace AuthUsersService.Infrastructure.Mappers;

internal static class UserExtensions
{
    public static UserEntity ToEntity(this User domain)
    {
        return new UserEntity
        {
            Id = domain.Id,
            Nickname = domain.Nickname,
            Email = domain.Email,
            PasswordHash = domain.PasswordHash,
            CreatedAt = domain.CreatedAt
        };
    }

    public static User ToDomain(this UserEntity entity)
    {
        var domain = User.Create(
            entity.Nickname,
            entity.Email,
            entity.PasswordHash,
            entity.CreatedAt);

        domain.SetId(entity.Id);
        return domain;
    }
}
