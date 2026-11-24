using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.UpdateComment;

public sealed record UpdateCommentCommand(
    long CommentId,
    long TopicId,
    long? ParentCommentId,
    long UpdatedByUserId,
    string Content,
    bool isAdmin,
    bool isModerator) : IRequest<int>;

internal sealed class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, int>
{
    private readonly ICommentRepository _commentRepository;

    public UpdateCommentCommandHandler(
        ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<int> Handle(
        UpdateCommentCommand command,
        CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(
            command.CommentId,
            cancellationToken);

        if (comment is null)
        {
            return -1;
        }

        // Перевешивание комментария в другую ветку не поддерживается
        comment.Update(
            command.Content,
            DateTimeOffset.UtcNow,
            command.UpdatedByUserId);

        return await _commentRepository.UpdateAsync(
            comment,
            cancellationToken);
    }
}
