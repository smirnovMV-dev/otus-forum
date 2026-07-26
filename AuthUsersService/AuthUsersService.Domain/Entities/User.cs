using System;

namespace AuthUsersService.Domain.Entities;

public sealed record User
{
    public long Id { get; private set; }
    public string Nickname { get; }
    public string Email { get; }    
    public string PasswordHash { get; }
    public DateTimeOffset CreatedAt { get; }    

    public static User Create(
        string nickname,
        string email,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        return new User(
            nickname,
            email,
            passwordHash,
            createdAt);
    }

    public long SetId(long id) => Id = id;
    
    private User(
        string nickname,
        string email,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        Nickname = nickname;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }
}
