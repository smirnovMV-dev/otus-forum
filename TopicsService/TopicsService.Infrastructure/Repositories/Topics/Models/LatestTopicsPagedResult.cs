using System.Collections.Generic;

namespace TopicsService.Infrastructure.Repositories.Topics.Models;

public sealed record LatestTopicsPagedResult(
    IReadOnlyCollection<LatestTopicsDto> Topics,
    int TotalCount);
