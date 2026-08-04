namespace AuthUsersService.Infrastructure.Repositories.Users.Dtos;

internal sealed record UserNicknameDto(
    long UserId,
    string Nickname);