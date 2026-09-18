using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Domain.Entities;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.UpdateTopic;

internal sealed class UpdateTopicCommandHandler : IRequestHandler<UpdateTopicCommand>
{
    private readonly ITopicsRepository _topicsRepository;

    public UpdateTopicCommandHandler(ITopicsRepository topicsRepository)
    {
        _topicsRepository = topicsRepository;
    }

    public async Task Handle(UpdateTopicCommand command, CancellationToken cancellationToken)
    {
        var topic = await _topicsRepository.GetByIdAsync(command.TopicId, cancellationToken);

        if (topic is null)
        {
            throw new Exception($"Тема с ID {command.TopicId} не найдена.");
        }

        if (topic.AuthorId != command.AuthorId)
        {
            throw new Exception("У вас нет прав для редактирования этой темы.");
        }

        topic.UpdateTitle(command.Title, DateTimeOffset.UtcNow);
        await _topicsRepository.UpdateAsync(topic, cancellationToken);
    }
}
