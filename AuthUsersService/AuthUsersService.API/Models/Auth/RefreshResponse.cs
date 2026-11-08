namespace AuthUsersService.API.Models.Auth;

public sealed class RefreshResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}
