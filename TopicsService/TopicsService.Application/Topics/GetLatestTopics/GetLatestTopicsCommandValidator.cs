using FluentValidation;

namespace TopicsService.Application.Topics.GetLatestTopics;

internal class GetLatestTopicsCommandValidator : AbstractValidator<GetLatestTopicsCommand>
{
    public GetLatestTopicsCommandValidator()
    {
        RuleFor(command => command)
            .NotNull()
            .WithMessage("Запрос не соответствует формату.");

        RuleFor(command => command.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize должно быть больше 0.");

        RuleFor(command => command.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page должно быть больше или равно 1.");
    }
}
