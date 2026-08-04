namespace AuthUsersService.Application.Users.GetNicknames.Models;

internal sealed record GetNicknamesResult(
    long UserId,
    string Nickname);
