using System;
using MediatR;

namespace TopicsService.Application.Topics.TopicCleaner;

public sealed record DeleteSoftDeletedTopicsCommand(
    DateTimeOffset CutoffDate,
    int BatchSize) : IRequest<int>;
