using System;

namespace TopicsService.Infrastructure.Repositories.Topics.Models;

public sealed record LatestTopicsDto(
    long Id,
    string Title,
    long AuthorId,
    DateTimeOffset CreatedAt);