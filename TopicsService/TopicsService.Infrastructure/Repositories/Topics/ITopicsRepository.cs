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

    Task<IReadOnlyCollection<LatestTopicsModel>> GetLatestTopicsAsync(
        int count,
        CancellationToken cancellationToken);
}
