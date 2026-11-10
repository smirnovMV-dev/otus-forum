using System;
using System.Collections.Generic;

namespace TopicsService.Application.Topics.GetTopics.Models;

public sealed record GetTopicsResult(
    IReadOnlyCollection<GetTopicsItem> Topics,
    int TotalCount);

public sealed record GetTopicsItem(
    long Id,
    string Title,
    string AuthorNikname,
    DateTimeOffset CreatedAt,
    int CommentsCount);
