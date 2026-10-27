using MediatR;

namespace TopicsService.Application.Topics.DeleteTopic;

public sealed record DeleteTopicCommand(
    long TopicId,
    long AuthorId,
    bool isAdmin,
    bool isModerator) : IRequest;
