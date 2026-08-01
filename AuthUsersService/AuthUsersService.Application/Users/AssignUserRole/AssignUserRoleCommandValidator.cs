using FluentValidation;

namespace AuthUsersService.Application.Users.AssignUserRole;

internal class AssignUserRoleCommandValidator : AbstractValidator<AssignUserRoleCommand>
{
    public AssignUserRoleCommandValidator(AssignUserRoleCommand command)
    {
        RuleFor(command => command.UserId)
            .Must(userId => userId > 0)
            .WithMessage("Id пользователя должен быть больше ноля.");
        RuleFor(command => command.RoleId)
            .Must(roleId => roleId > 0)
            .WithMessage("Id роли должен быть больше ноля.");
    }
}
