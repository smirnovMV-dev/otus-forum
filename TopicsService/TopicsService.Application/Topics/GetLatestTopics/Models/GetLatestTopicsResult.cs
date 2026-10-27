using System;
using System.Collections.Generic;

namespace TopicsService.Application.Topics.GetLatestTopics.Models;

public sealed record GetLatestTopicsResult(
    IReadOnlyCollection<GetLatestTopicsItem> Topics,
    int TotalCount);

public sealed record GetLatestTopicsItem(
    long Id,
    string Title,
    string AuthorNikname,
    DateTimeOffset CreatedAt);
