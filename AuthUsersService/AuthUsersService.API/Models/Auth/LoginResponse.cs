namespace AuthUsersService.API.Models.Auth;

public sealed class LoginResponse
{
    public required string Token { get; set; }
    public required long UserId { get; set; }
    public required string Nickname { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsModerator { get; set; }
}
