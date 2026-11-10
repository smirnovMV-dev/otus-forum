namespace AuthUsersService.API.Models.Auth;

public sealed class LoginRequest
{
    public required string Nickname { get; set; }
    public required string Password { get; set; }
}
