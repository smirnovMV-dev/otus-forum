using FluentValidation;

namespace TopicsService.Application.Topics.UpdateTopic;

public sealed class UpdateTopicCommandValidator : AbstractValidator<UpdateTopicCommand>
{
    public UpdateTopicCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .WithMessage("Заголовок топика не может быть пустым.");

        RuleFor(command => command.TopicId)
            .GreaterThan(0L)
            .WithMessage("ID топика должен быть больше нуля.");

        RuleFor(command => command.AuthorId)
            .GreaterThan(0L)
            .WithMessage("ID автора топика должен быть больше нуля.");
    }
}
