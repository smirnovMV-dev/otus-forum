using FluentValidation;

namespace CommentsService.Application.Comments.UpdateComment;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .GreaterThan(0)
            .WithMessage("CommentId должен быть больше 0.");

        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .WithMessage("TopicId должен быть больше 0.");

        RuleFor(x => x.UpdatedByUserId)
            .GreaterThan(0)
            .WithMessage("UpdatedByUserId должен быть больше 0.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content обязателен.")
            .MaximumLength(5000)
            .WithMessage("Content не должен превышать 5000 символов.");
    }
}
