using System;
using System.Collections.Generic;

namespace TopicsService.Application.Topics.GetTopicsByUserId.Models;

public sealed record GetTopicsByUserIdResult(
    IReadOnlyCollection<GetTopicsByUserIdItem> Topics,
    int TotalCount);

public sealed record GetTopicsByUserIdItem(
    long Id,
    string Title,
    string AuthorNikname,
    DateTimeOffset CreatedAt);
