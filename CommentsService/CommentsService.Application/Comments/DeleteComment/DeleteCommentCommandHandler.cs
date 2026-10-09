using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.DeleteComment;

public sealed record DeleteCommentCommand(
    long CommentId) : IRequest<int>;

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
        return await _commentRepository.SoftDeleteAsync(
            command.CommentId,
            DateTimeOffset.UtcNow,
            cancellationToken);
    }
}
