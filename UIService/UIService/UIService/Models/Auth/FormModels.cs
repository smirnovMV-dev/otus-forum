using System.ComponentModel.DataAnnotations;

namespace UIService.Models.Auth;

public class LoginFormModel
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class UserRegisterFormModel
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
