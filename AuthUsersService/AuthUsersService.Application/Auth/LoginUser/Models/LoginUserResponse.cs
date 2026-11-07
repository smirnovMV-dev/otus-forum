namespace AuthUsersService.Application.Auth.LoginUser.Models;

public sealed record LoginUserResponse(
    string Token,
    string RefreshToken,
    long UserId,
    string Nickname,
    bool IsAdmin,
    bool IsModerator);
