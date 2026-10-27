using MediatR;
using TopicsService.Application.Topics.GetTopicWithComments.Models;

namespace TopicsService.Application.Topics.GetTopicWithComments;

public sealed record GetTopicWithCommentsCommand(
    long TopicId,
    long UserId,
    bool IsAdmin,
    bool IsModerator)
    : IRequest<GetTopicWithCommentsResult>;
