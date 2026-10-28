namespace CommentsService.Application.Services.Externals.UsersApiService.Models;

internal sealed record UserNickname(
    long UserId,
    string Nickname);
