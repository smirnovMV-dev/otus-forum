using System;

namespace TopicsService.API.Models.Topics.GetLatestTopics;

public sealed class LatestTopicsResponse
{
    public required long Id { get; set; }

    public required string Title { get; set; }

    public required string AuthorNikname { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }
}
