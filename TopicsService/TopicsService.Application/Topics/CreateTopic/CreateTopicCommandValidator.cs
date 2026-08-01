using FluentValidation;

namespace TopicsService.Application.Topics.CreateTopic;

public sealed class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(command => command.Caption)
            .NotEmpty()
            .WithMessage("Заголовок топика не может быть пустым.");
        RuleFor(command => command.AuthorId)
            .GreaterThan(0L)
            .WithMessage("Id автора топика должнен быть больше ноля.");
    }
}
