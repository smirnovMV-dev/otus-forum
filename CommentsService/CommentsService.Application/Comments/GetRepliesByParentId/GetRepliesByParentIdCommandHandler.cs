using CommentsService.Application.Comments.GetCommentsByTopic.Models;
using CommentsService.Application.Comments.GetRepliesByParentId.Models;
using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.GetRepliesByParentId;

internal sealed class GetRepliesByParentIdCommandHandler
    : IRequestHandler<GetRepliesByParentIdCommand, GetRepliesByParentIdResult>
{
    private readonly ICommentRepository _commentRepository;

    public GetRepliesByParentIdCommandHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<GetRepliesByParentIdResult> Handle(
        GetRepliesByParentIdCommand command,
        CancellationToken cancellationToken)
    {
        var page = await _commentRepository.GetPagedRepliesAsync(
            command.ParentCommentId,
            command.Page,
            command.PageSize,
            cancellationToken);

        var replies = page.Replies
            .Select(reply => new CommentTreeDto(
                reply.Id,
                reply.TopicId,
                reply.ParentCommentId,
                reply.AuthorId,
                reply.ContentDisplay,
                reply.CreatedAt.ToString("o"),
                reply.UpdatedAt.ToString("o"),
                Array.Empty<CommentTreeDto>(),
                page.ReplyCounts.GetValueOrDefault(reply.Id)))
            .ToList();

        return new GetRepliesByParentIdResult(replies, page.TotalCount);
    }
}
