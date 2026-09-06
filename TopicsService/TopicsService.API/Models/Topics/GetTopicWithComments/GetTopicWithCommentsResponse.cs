using System;
using System.Collections.Generic;

namespace TopicsService.API.Models.Topics.GetTopicWithComments;

public sealed class GetTopicWithCommentsResponse
{
    public required long TopicId { get; set; }
    public required string Title { get; set; }
    public required long AuthorId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required IReadOnlyCollection<CommentResponse> Comments { get; set; }
}

public sealed class CommentResponse
{
    public required long Id { get; set; }
    public required long TopicId { get; set; }
    public required long? ParentCommentId { get; set; }
    public required string AuthorNickname { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
