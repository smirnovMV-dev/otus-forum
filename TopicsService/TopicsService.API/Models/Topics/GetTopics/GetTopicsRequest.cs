namespace TopicsService.API.Models.Topics.GetTopics;

public sealed class GetTopicsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}
