namespace AuthUsersService.API.Models.Auth;

public sealed class LoginResponse
{
    public required string Token { get; set; }
    public required long UserId { get; set; }
    public required string Nickname { get; set; }
}
