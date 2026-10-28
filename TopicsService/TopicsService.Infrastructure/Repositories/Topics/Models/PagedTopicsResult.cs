using System.Collections.Generic;

namespace TopicsService.Infrastructure.Repositories.Topics.Models;

public sealed record PagedTopicsResult(
    IReadOnlyCollection<LatestTopicsDto> Topics,
    int TotalCount);
