using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.TopicCleaner;

internal sealed class DeleteSoftDeletedTopicsCommandHandler(
    ITopicsRepository topicsRepository)
    : IRequestHandler<DeleteSoftDeletedTopicsCommand, int>
{
    public async Task<int> Handle(
        DeleteSoftDeletedTopicsCommand command,
        CancellationToken cancellationToken)
    {
        return await topicsRepository.DeleteBatchAsync(
            command.CutoffDate,
            command.BatchSize,
            cancellationToken);
    }
}
