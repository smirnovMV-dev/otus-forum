using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.DeleteTopic;

internal sealed class DeleteTopicCommandHandler : IRequestHandler<DeleteTopicCommand>
{
    private readonly ITopicsRepository _topicsRepository;

    public DeleteTopicCommandHandler(
        ITopicsRepository topicsRepository)
    {
        _topicsRepository = topicsRepository;
    }

    public async Task Handle(DeleteTopicCommand command, CancellationToken cancellationToken)
    {
        var topic = await _topicsRepository.GetByIdAsync(command.TopicId, cancellationToken);

        if (topic is null)
        {
            throw new Exception($"Тема с ID {command.TopicId} не найдена.");
        }

        var canDelete = topic.AuthorId == command.AuthorId
            || command.isAdmin
            || command.isModerator;

        if (!canDelete)
        {
            throw new Exception("У вас нет прав для удаления этой темы.");
        }

        var deletedAt = DateTimeOffset.UtcNow;
        topic.MarkAsDeleted(deletedAt);
        await _topicsRepository.UpdateAsync(topic, cancellationToken);
    }
}
