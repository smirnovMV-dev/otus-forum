using CommentsService.Domain.Entities;
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
    string Content) : IRequest<int>;

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
        var comment = Comment.Create(
            topicId: 0,
            parentCommentId: null,
            command.UpdatedByUserId,
            command.Content,
            DateTimeOffset.UtcNow);

        comment.SetId(command.CommentId);
        comment.Update(command.TopicId, command.ParentCommentId, command.Content, DateTimeOffset.UtcNow, command.UpdatedByUserId);

        return await _commentRepository.UpdateAsync(
            comment,
            cancellationToken);
    }
}
