using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommentsService.Domain.Entities;
using CommentsService.Infrastructure.Repositories.Comments.Models;

namespace CommentsService.Infrastructure.Repositories.Comments;

public interface ICommentRepository
{
    Task<int> CreateAsync(
        Comment comment,
        CancellationToken cancellationToken);

    Task<int> UpdateAsync(
        Comment comment,
        CancellationToken cancellationToken);

    Task<int> SoftDeleteAsync(
        long commentId,
        DateTimeOffset deletedAt,
        CancellationToken cancellationToken);

    Task<Comment?> GetByIdAsync(
        long commentId,
        CancellationToken cancellationToken);

    Task<PagedCommentsResult> GetByUserIdAsync(
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<CommentTreePage> GetCommentTreePageAsync(
        long topicId,
        int page,
        int pageSize,
        int repliesPreviewSize,
        CancellationToken cancellationToken);

    Task<PagedRepliesResult> GetPagedRepliesAsync(
        long parentCommentId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<int> CountAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, int>> CountByTopicIdsAsync(
        IReadOnlyCollection<long> topicIds,
        CancellationToken cancellationToken);
}
