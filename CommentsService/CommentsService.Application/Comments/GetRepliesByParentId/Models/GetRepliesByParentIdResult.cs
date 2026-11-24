using System.Collections.Generic;
using CommentsService.Application.Comments.GetCommentsByTopic.Models;

namespace CommentsService.Application.Comments.GetRepliesByParentId.Models;

public sealed record GetRepliesByParentIdResult(
    IReadOnlyCollection<CommentTreeDto> Replies,
    int TotalCount);
