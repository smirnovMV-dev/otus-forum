namespace TopicsService.API.Models.Topics.GetTopicsByUserId;

public sealed class GetTopicsByUserIdRequest
{
    public required long UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
