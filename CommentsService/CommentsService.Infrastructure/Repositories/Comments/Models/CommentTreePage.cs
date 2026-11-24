using System.Collections.Generic;
using CommentsService.Domain.Entities;

namespace CommentsService.Infrastructure.Repositories.Comments.Models;

public sealed record CommentTreePage(
    IReadOnlyCollection<Comment> RootComments,
    IReadOnlyCollection<Comment> PreviewReplies,
    IReadOnlyDictionary<long, int> ReplyCounts,
    int TotalRootCount);

public sealed record PagedRepliesResult(
    IReadOnlyCollection<Comment> Replies,
    IReadOnlyDictionary<long, int> ReplyCounts,
    int TotalCount);
