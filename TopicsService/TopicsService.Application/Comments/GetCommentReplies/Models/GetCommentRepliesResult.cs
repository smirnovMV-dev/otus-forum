using System.Collections.Generic;
using TopicsService.Application.Topics.GetTopicWithComments.Models;

namespace TopicsService.Application.Comments.GetCommentReplies.Models;

public sealed record GetCommentRepliesResult(
    IReadOnlyCollection<CommentResult> Comments,
    int TotalCount);
