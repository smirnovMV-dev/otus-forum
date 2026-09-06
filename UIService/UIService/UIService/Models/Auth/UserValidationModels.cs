using System.ComponentModel.DataAnnotations;

namespace UIService.Models.Auth;

public sealed class LoginFormModel
{
    [Required(ErrorMessage = "Email обязателен для заполнения")]
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Email должен содержать от 3 до 100 символов")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен для заполнения")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 100 символов")]
    public string Password { get; set; } = string.Empty;
}

public sealed class UserRegisterFormModel
{
    [Required(ErrorMessage = "Ник обязателен для заполнения")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Ник должен содержать от 3 до 100 символов")]
    public string Nickname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен для заполнения")]
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Email должен содержать от 3 до 100 символов")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен для заполнения")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 100 символов")]
    public string Password { get; set; } = string.Empty;
}
