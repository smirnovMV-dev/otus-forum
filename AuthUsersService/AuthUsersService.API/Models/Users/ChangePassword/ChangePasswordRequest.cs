namespace AuthUsersService.API.Models.Users.ChangePassword;

public sealed class ChangePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
