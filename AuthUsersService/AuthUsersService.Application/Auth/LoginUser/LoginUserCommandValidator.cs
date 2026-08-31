using FluentValidation;

namespace AuthUsersService.Application.Auth.LoginUser;

internal sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email не должен быть пустым.")
            .EmailAddress()
            .WithMessage("Email должен быть валидным.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Пароль не должен быть пустым.");
    }
}
