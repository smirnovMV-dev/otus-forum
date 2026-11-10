using System;

namespace TopicsService.API.Models.Topics.GetTopics;

public sealed class TopicsItemResponse
{
    public required long Id { get; set; }
    public required string Title { get; set; }
    public required string AuthorNikname { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required int CommentsCount { get; set; }
}
