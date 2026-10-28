using System.Collections.Generic;
using TopicsService.API.Models.Topics.GetLatestTopics;

namespace TopicsService.API.Models.Topics.GetTopicsByUserId;

public sealed class GetTopicsByUserIdResponse
{
    public required IReadOnlyCollection<LatestTopicsResponse> Topics { get; set; }
    public required int TotalCount { get; set; }
}
