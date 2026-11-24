using CommentsService.Application.Comments.GetCommentsByTopic.Models;
using CommentsService.Domain.Entities;
using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.GetCommentsByTopic;

internal sealed class GetCommentsByTopicCommandHandler
    : IRequestHandler<GetCommentsByTopicCommand, GetCommentsByTopicResult>
{
    // Ответы, отдаваемые вместе с корневым комментарием до подгрузки следующих страниц.
    private const int RepliesPreviewSize = 5;

    private readonly ICommentRepository _commentRepository;

    public GetCommentsByTopicCommandHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<GetCommentsByTopicResult> Handle(
        GetCommentsByTopicCommand command,
        CancellationToken cancellationToken)
    {
        var page = await _commentRepository.GetCommentTreePageAsync(
            command.TopicId,
            command.Page,
            command.PageSize,
            RepliesPreviewSize,
            cancellationToken);

        var repliesByParentId = page.PreviewReplies
            .GroupBy(r => r.ParentCommentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var comments = page.RootComments
            .Select(root => MapToDto(
                root,
                repliesByParentId.TryGetValue(root.Id, out var replies)
                    ? replies
                    : Array.Empty<Comment>(),
                page.ReplyCounts))
            .ToList();

        return new GetCommentsByTopicResult(comments, page.TotalRootCount);
    }

    private static CommentTreeDto MapToDto(
        Comment comment,
        IReadOnlyCollection<Comment> replies,
        IReadOnlyDictionary<long, int> replyCounts)
    {
        var replyDtos = replies
            .Select(reply => MapToDto(reply, Array.Empty<Comment>(), replyCounts))
            .ToList();

        return new CommentTreeDto(
            comment.Id,
            comment.TopicId,
            comment.ParentCommentId,
            comment.AuthorId,
            comment.ContentDisplay,
            comment.CreatedAt.ToString("o"),
            comment.UpdatedAt.ToString("o"),
            replyDtos,
            replyCounts.GetValueOrDefault(comment.Id));
    }
}
