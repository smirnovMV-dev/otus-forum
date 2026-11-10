using System.Collections.Generic;

namespace TopicsService.API.Models.Topics.GetTopics;

public sealed class GetTopicsResponse
{
    public required IReadOnlyCollection<TopicsItemResponse> Topics { get; set; }
    public required int TotalCount { get; set; }
}
