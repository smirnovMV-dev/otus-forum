namespace TopicsService.API.Models.Topics.GetLatestTopics;

public sealed class GetLatestTopicsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}
