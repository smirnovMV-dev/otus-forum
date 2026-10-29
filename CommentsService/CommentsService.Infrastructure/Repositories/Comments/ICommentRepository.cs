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

    Task<PagedCommentsResult> GetByUserIdAsync(
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<(IReadOnlyCollection<Comment> RootComments, IReadOnlyCollection<Comment> ChildComments)> GetPagedCommentTreeAsync(
        long topicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
