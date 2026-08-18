using FluentValidation;

namespace CommentsService.Application.Comments.CreateComment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .WithMessage("TopicId должен быть больше 0.");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0)
            .WithMessage("AuthorId должен быть больше 0.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content обязателен.")
            .MaximumLength(5000)
            .WithMessage("Content не должен превышать 5000 символов.");
    }
}
