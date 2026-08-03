using FluentValidation;

namespace TopicsService.Application.Topics.GetLatestTopics;

internal class GetLatestTopicsCommandValidator : AbstractValidator<GetLatestTopicsCommand>
{
    public GetLatestTopicsCommandValidator()
    {
        RuleFor(command => command)
            .NotNull()
            .WithMessage("Запрос не соответствует формату.");
    }
}
