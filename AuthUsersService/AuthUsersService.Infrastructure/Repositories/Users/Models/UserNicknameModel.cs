using AuthUsersService.Infrastructure.Repositories.Users.Dtos;

namespace AuthUsersService.Infrastructure.Repositories.Users.Models;

public sealed record UserNicknameModel
{
    public long UserId { get; }

    public string Nickname { get; }

    internal static UserNicknameModel Create(
        UserNicknameDto dto)
    {
        return new UserNicknameModel(dto.UserId, dto.Nickname);
    }

    private UserNicknameModel(
        long userId,
        string nickname)
    {
        UserId = userId;
        Nickname = nickname;
    }
}
