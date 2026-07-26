using System;

namespace AuthUsersService.Domain.Entities;

public sealed record Role
{
    public long Id { get; private set; }
    public string Name { get; }
    public DateTimeOffset CreatedAt { get; }

    public static Role Create(
        string name,
        DateTimeOffset createdAt)
    {
        return new Role(
            name,
            createdAt);
    }

    public long SetId(long id) => Id = id;

    private Role(
        string name,
        DateTimeOffset createdAt)
    {
        Name = name;
        CreatedAt = createdAt;
    }
}
