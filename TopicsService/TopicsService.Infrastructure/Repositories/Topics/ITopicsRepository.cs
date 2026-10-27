using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Domain.Entities;
using TopicsService.Infrastructure.Repositories.Topics.Models;

namespace TopicsService.Infrastructure.Repositories.Topics;

public interface ITopicsRepository
{
    Task<int> CreateAsync(
        Topic topic,
        CancellationToken cancellationToken);

    Task<LatestTopicsPagedResult> GetLatestTopicsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Topic?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Topic topic,
        CancellationToken cancellationToken);

    Task<int> DeleteBatchAsync(
        DateTimeOffset cutoffDate,
        int batchSize,
        CancellationToken cancellationToken);
}
