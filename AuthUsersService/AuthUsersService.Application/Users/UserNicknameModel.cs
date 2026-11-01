namespace AuthUsersService.Application.Users;

public sealed record UserNicknameModel
{
    public long UserId { get; }
    public string Nickname { get; }

    public UserNicknameModel(long userId, string nickname)
    {
        UserId = userId;
        Nickname = nickname;
    }
}
