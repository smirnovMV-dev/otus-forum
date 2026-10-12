using FluentValidation;

namespace TopicsService.Application.Topics.DeleteTopic;

public sealed class DeleteTopicCommandValidator : AbstractValidator<DeleteTopicCommand>
{
    public DeleteTopicCommandValidator()
    {
        RuleFor(command => command.TopicId)
            .GreaterThan(0L)
            .WithMessage("ID топика должен быть больше нуля.");

        RuleFor(command => command.AuthorId)
            .GreaterThan(0L)
            .WithMessage("ID автора топика должен быть больше нуля.");
    }
}
