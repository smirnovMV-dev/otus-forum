namespace TopicsService.API.Models.Topics.CreateTopic;

public sealed class CreateTopicRequest
{
    public string Caption { get; set; } = string.Empty;

    public long AuthorId { get; set; }
}
