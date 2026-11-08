namespace AuthUsersService.API.Models.Auth;

public sealed class MeResponse
{
    public bool IsAuthenticated { get; set; }
    public long UserId { get; set; }
    public string? Nickname { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsModerator { get; set; }
}
