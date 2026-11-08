using CommentsService.Domain.Exceptions;
using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.DeleteComment;

public sealed record DeleteCommentCommand(
    long CommentId,
    long UserId,
    bool IsAdmin,
    bool IsModerator) : IRequest<int>;

internal sealed class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, int>
{
    private readonly ICommentRepository _commentRepository;

    public DeleteCommentCommandHandler(
        ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<int> Handle(
        DeleteCommentCommand command,
        CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(command.CommentId, cancellationToken);

        if (comment is null)
        {
            return -1;
        }

        var canDelete = command.IsAdmin
            || command.IsModerator
            || comment.AuthorId == command.UserId;

        if (!canDelete)
        {
            throw new ForbiddenOperationException("Удалять можно только свои комментарии.");
        }

        return await _commentRepository.SoftDeleteAsync(
            command.CommentId,
            DateTimeOffset.UtcNow,
            cancellationToken);
    }
}
