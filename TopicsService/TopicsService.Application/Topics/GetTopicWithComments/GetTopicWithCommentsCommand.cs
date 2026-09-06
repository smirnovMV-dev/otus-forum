using MediatR;
using TopicsService.Application.Topics.GetTopicWithComments.Models;

namespace TopicsService.Application.Topics.GetTopicWithComments;

public sealed record GetTopicWithCommentsCommand(long TopicId)
    : IRequest<GetTopicWithCommentsResult>;
