using FluentValidation;

namespace AuthUsersService.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(RegisterUserCommand command)
    {
        RuleFor(command => command.Nickname)
            .NotEmpty()
            .WithMessage("Ник не должен быть пустым.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Почта не должена быть пустой.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Пароль не должен быть пустым.");
    }
}
