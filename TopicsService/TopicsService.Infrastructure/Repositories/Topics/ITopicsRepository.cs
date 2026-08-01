using System.Threading;
using System.Threading.Tasks;
using TopicsService.Domain.Entities;

namespace TopicsService.Infrastructure.Repositories.Topics;

public interface ITopicsRepository
{
    Task<int> CreateAsync(
        Topic topic,
        CancellationToken cancellationToken);
}
