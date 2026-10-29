using CommentsService.Application.Comments.GetCommentsByTopic.Models;
using CommentsService.Domain.Entities;
using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.GetCommentsByTopic;

internal sealed class GetCommentsByTopicCommandHandler
    : IRequestHandler<GetCommentsByTopicCommand, GetCommentsByTopicResult>
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentsByTopicCommandHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<GetCommentsByTopicResult> Handle(
        GetCommentsByTopicCommand command,
        CancellationToken cancellationToken)
    {
        var (rootComments, childComments) = await _commentRepository.GetPagedCommentTreeAsync(
            command.TopicId,
            command.Page,
            command.PageSize,
            cancellationToken);

        var allCommentsList = new List<Comment>(rootComments);
        allCommentsList.AddRange(childComments);

        // Create DTOs
        var dtoMap = new Dictionary<long, CommentTreeDto>();
        foreach (var comment in allCommentsList)
        {
            dtoMap[comment.Id] = new CommentTreeDto(
                comment.Id,
                comment.TopicId,
                comment.ParentCommentId,
                comment.AuthorId,
                comment.ContentDisplay,
                comment.CreatedAt.ToString("o"),
                comment.UpdatedAt.ToString("o"),
                new List<CommentTreeDto>());
        }

        // Link comments to their parents
        var resultRoots = new List<CommentTreeDto>();
        foreach (var comment in allCommentsList)
        {
            var dto = dtoMap[comment.Id];
            if (comment.ParentCommentId == null)
            {
                resultRoots.Add(dto);
            }
            else if (dtoMap.TryGetValue(comment.ParentCommentId.Value, out var parent))
            {
                ((List<CommentTreeDto>)parent.Replies).Add(dto);
            }
        }

        // Sort replies chronologically (newest first) within each branch
        foreach (var root in resultRoots)
        {
            SortRepliesChronologically(root);
        }

        return new GetCommentsByTopicResult(resultRoots, rootComments.Count);
    }

    private void SortRepliesChronologically(CommentTreeDto comment)
    {
        var repliesList = (List<CommentTreeDto>)comment.Replies;
        repliesList.Sort((a, b) =>
        {
            var cmp = string.CompareOrdinal(a.CreatedAt, b.CreatedAt);
            return cmp != 0 ? cmp : a.Id.CompareTo(b.Id);
        });
        foreach (var reply in repliesList)
        {
            SortRepliesChronologically(reply);
        }
    }
}
