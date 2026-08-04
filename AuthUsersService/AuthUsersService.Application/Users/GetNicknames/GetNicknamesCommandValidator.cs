using FluentValidation;

namespace AuthUsersService.Application.Users.GetNicknames;

internal sealed class GetNicknamesCommandValidator : AbstractValidator<GetNicknamesCommand>
{
    public GetNicknamesCommandValidator()
    {
        RuleFor(command => command.UserIds)
            .NotEmpty()
            .WithMessage("Список Id пользователей не должен быть пустым.");
    }
}
