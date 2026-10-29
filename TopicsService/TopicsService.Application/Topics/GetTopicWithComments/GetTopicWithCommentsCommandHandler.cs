using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TopicsService.Application.Services.Externals.CommentsApiService;
using TopicsService.Application.Services.Externals.CommentsApiService.Models;
using TopicsService.Application.Services.Externals.UsersApiService;
using TopicsService.Application.Services.Externals.UsersApiService.Models;
using TopicsService.Application.Topics.GetTopicWithComments.Models;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.GetTopicWithComments;

internal sealed class GetTopicWithCommentsCommandHandler
    : IRequestHandler<GetTopicWithCommentsCommand, GetTopicWithCommentsResult>
{
    private readonly ITopicsRepository _topicsRepository;
    private readonly ICommentsApiServiceAdapter _commentsApiServiceAdapter;
    private readonly IUsersApiServiceAdapter _usersApiServiceAdapter;
    private readonly ILogger<GetTopicWithCommentsCommandHandler> _logger;

    public GetTopicWithCommentsCommandHandler(
        ITopicsRepository topicsRepository,
        ICommentsApiServiceAdapter commentsApiServiceAdapter,
        IUsersApiServiceAdapter usersApiServiceAdapter,
        ILogger<GetTopicWithCommentsCommandHandler> logger)
    {
        _topicsRepository = topicsRepository;
        _commentsApiServiceAdapter = commentsApiServiceAdapter;
        _usersApiServiceAdapter = usersApiServiceAdapter;
        _logger = logger;
    }

    public async Task<GetTopicWithCommentsResult> Handle(
        GetTopicWithCommentsCommand command,
        CancellationToken cancellationToken)
    {
        var topic = await _topicsRepository.GetByIdAsync(command.TopicId, cancellationToken);
        if (topic is null)
        {
            _logger.LogWarning("Тема с ID {TopicId} не найдена", command.TopicId);
            throw new KeyNotFoundException($"Тема с ID {command.TopicId} не найдена");
        }

        var comments = await _commentsApiServiceAdapter
            .GetCommentsByTopicIdAsync(command.TopicId, command.Page, command.PageSize, cancellationToken);

        var authorIds = comments.Select(c => c.AuthorId).Distinct().ToHashSet();
        var nicknames = await _usersApiServiceAdapter.GetUsersNicknamesAsync(authorIds, cancellationToken);

        var commentResults = comments.Select(c => MapToCommentResult(c, nicknames, command)).ToList();

        var canEdit = command.UserId == topic.AuthorId || command.IsAdmin || command.IsModerator;
        var canDelete = command.UserId == topic.AuthorId || command.IsAdmin || command.IsModerator;

        return new GetTopicWithCommentsResult(
            topic.Id,
            topic.Title,
            topic.AuthorId,
            topic.CreatedAt,
            commentResults,
            canEdit,
            canDelete);
    }

    private CommentResult MapToCommentResult(
        CommentDto dto,
        IReadOnlyDictionary<long, UserNickname> nicknames,
        GetTopicWithCommentsCommand command)
    {
        var commentCanEdit = command.UserId == dto.AuthorId || command.IsAdmin || command.IsModerator;
        var commentCanDelete = command.UserId == dto.AuthorId || command.IsAdmin || command.IsModerator;

        return new CommentResult(
            dto.Id,
            dto.TopicId,
            dto.ParentCommentId,
            nicknames.TryGetValue(dto.AuthorId, out var nick) ? nick.Nickname : "Неизвестный пользователь",
            dto.Content,
            DateTimeOffset.Parse(dto.CreatedAt),
            commentCanEdit,
            commentCanDelete,
            dto.Replies.Select(r => MapToCommentResult(r, nicknames, command))
                .ToList() as IReadOnlyCollection<CommentResult>);
    }
}
