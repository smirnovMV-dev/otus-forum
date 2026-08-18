using CommentsService.Domain.Entities;
using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.CreateComment;

public sealed record CreateCommentCommand(
    long TopicId,
    long? ParentCommentId,
    long AuthorId,
    string Content) : IRequest<int>;

internal sealed class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, int>
{
    private readonly ICommentRepository _commentRepository;

    public CreateCommentCommandHandler(
        ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<int> Handle(
        CreateCommentCommand command,
        CancellationToken cancellationToken)
    {
        var comment = Comment.Create(
            command.TopicId,
            command.ParentCommentId,
            command.AuthorId,
            command.Content,
            DateTimeOffset.UtcNow);

        return await _commentRepository.CreateAsync(
            comment,
            cancellationToken);
    }
}
