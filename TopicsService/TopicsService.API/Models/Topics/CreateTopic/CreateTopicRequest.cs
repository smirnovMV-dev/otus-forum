namespace TopicsService.API.Models.Topics.CreateTopic;

public sealed class CreateTopicRequest
{
    public required string Title { get; set; }

    public required long AuthorId { get; set; }
}
