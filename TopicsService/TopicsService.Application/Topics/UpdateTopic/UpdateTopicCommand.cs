using MediatR;

namespace TopicsService.Application.Topics.UpdateTopic;

public sealed record UpdateTopicCommand(
    long TopicId,
    string Title,
    long AuthorId,
    bool isAdmin,
    bool isModerator) : IRequest;
