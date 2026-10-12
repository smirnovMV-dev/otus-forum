using MediatR;

namespace TopicsService.Application.Topics.DeleteTopic;

public sealed record DeleteTopicCommand(
    long TopicId,
    long AuthorId) : IRequest;
