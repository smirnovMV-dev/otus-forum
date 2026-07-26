using System;

namespace AuthUsersService.Domain.Entities;

public sealed record UserRole
{
    public long UserId { get; }
    public long RoleId { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ExpiresAt { get; }

    public static UserRole Create(
        long userId,
        long roleId,
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt)
        => new UserRole(
            userId,
            roleId,
            createdAt,
            expiresAt);

    private UserRole(
        long userId,
        long roleId,
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt)
    {
        UserId = userId;
        RoleId = roleId;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }
}
