using TopicsService.Application.Topics.GetTopics.Models;
using MediatR;

namespace TopicsService.Application.Topics.GetTopics;

public sealed record GetTopicsCommand(
    int Page,
    int PageSize) : IRequest<GetTopicsResult>;
