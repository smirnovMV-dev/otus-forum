using MediatR;
using TopicsService.Application.Topics.GetTopicsByUserId.Models;

namespace TopicsService.Application.Topics.GetTopicsByUserId;

public sealed record GetTopicsByUserIdCommand(
    long UserId,
    int Page,
    int PageSize) : IRequest<GetTopicsByUserIdResult>;
