using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Domain.Entities;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.CreateTopic;

public sealed record CreateTopicCommand(
    string Title,
    long AuthorId) : IRequest<int>;

internal class CreateTopicCommandHandler : IRequestHandler<CreateTopicCommand, int>
{
    private readonly ITopicsRepository _topicsRepository;

    public CreateTopicCommandHandler(
        ITopicsRepository topicsRepository)
    {
        _topicsRepository = topicsRepository;
    }

    public async Task<int> Handle(
        CreateTopicCommand command,
        CancellationToken cancellationToken)
    {
        var topic = Topic.Create(
            command.Title,
            command.AuthorId,
            DateTimeOffset.UtcNow);

        return await _topicsRepository.CreateAsync(
            topic,
            cancellationToken);
    }
}
