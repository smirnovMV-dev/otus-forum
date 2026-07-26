using FluentValidation;

namespace AuthUsersService.Application.Roles.CreateRole;

internal class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(CreateRoleCommand command)
    {
        RuleFor(command => command.RoleName)
            .NotEmpty()
            .WithMessage("Нименование роли не должно быть пустым.");
    }
}
