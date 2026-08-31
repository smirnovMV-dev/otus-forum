namespace AuthUsersService.Application.Auth.LoginUser.Models;

public sealed record LoginUserResponse(
    string Token,
    long UserId,
    string Nickname);
