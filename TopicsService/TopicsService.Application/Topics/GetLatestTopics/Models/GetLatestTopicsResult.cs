using System;

namespace TopicsService.Application.Topics.GetLatestTopics.Models;

public sealed record GetLatestTopicsResult(
    long Id,
    string Title,
    string AuthorNikname,
    DateTimeOffset CreatedAt);
