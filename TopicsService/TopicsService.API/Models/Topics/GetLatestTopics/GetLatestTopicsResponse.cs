using System.Collections.Generic;

namespace TopicsService.API.Models.Topics.GetLatestTopics;

public sealed class GetLatestTopicsResponse
{
    public required IReadOnlyCollection<LatestTopicsResponse> LatestTopics { get; set; }
}
