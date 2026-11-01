using System;
using AuthUsersService.Domain;

namespace AuthUsersService.Infrastructure.Data;

internal sealed class UserEntity
{
    public long Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

internal sealed class RoleEntity
{
    public long Id { get; set; }
    public RoleName Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

internal sealed class UserRoleEntity
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}
