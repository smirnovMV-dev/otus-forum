using FluentValidation;

namespace AuthUsersService.Application.Auth.LoginUser;

internal sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(command => command.Nickname)
            .NotEmpty()
            .WithMessage("Никнейм не должен быть пустым.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Пароль не должен быть пустым.");
    }
}
