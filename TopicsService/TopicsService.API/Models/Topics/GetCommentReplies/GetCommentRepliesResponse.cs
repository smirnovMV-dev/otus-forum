using System.Collections.Generic;
using TopicsService.API.Models.Topics.GetTopicWithComments;

namespace TopicsService.API.Models.Topics.GetCommentReplies;

public sealed class GetCommentRepliesResponse
{
    public required IReadOnlyCollection<CommentResponse> Comments { get; set; }
    public required int TotalCount { get; set; }
}
