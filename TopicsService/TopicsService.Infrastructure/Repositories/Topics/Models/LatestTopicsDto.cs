using System;

namespace TopicsService.Infrastructure.Repositories.Topics.Models;

internal sealed record LatestTopicsDto(
    long Id,
    string Title,
    long AuthorId,
    DateTimeOffset CreatedAt);