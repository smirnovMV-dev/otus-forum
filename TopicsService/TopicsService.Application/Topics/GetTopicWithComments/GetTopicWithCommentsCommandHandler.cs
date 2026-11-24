using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TopicsService.Application.Comments;
using TopicsService.Application.Services.Externals.CommentsApiService;
using TopicsService.Application.Services.Externals.CommentsApiService.Models;
using TopicsService.Application.Services.Externals.UsersApiService;
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

        var commentsPage = await _commentsApiServiceAdapter
            .GetCommentsByTopicIdAsync(command.TopicId, command.Page, command.PageSize, cancellationToken);

        var allAuthors = new HashSet<long>();
        void CollectAuthorIds(IEnumerable<CommentDto> items)
        {
            foreach (var c in items)
            {
                allAuthors.Add(c.AuthorId);
                if (c.Replies.Any()) CollectAuthorIds(c.Replies);
            }
        }
        CollectAuthorIds(commentsPage.Comments);
        var authorIds = allAuthors.ToHashSet();
        authorIds.Add(topic.AuthorId);
        var nicknames = await _usersApiServiceAdapter.GetUsersNicknamesAsync(authorIds, cancellationToken);

        var commentResults = commentsPage.Comments
            .Select(c => CommentResultMapper.Map(c, nicknames, command.UserId, command.IsAdmin, command.IsModerator))
            .ToList();

        var topicAuthorNickname = nicknames.TryGetValue(topic.AuthorId, out var topicNick)
            ? topicNick.Nickname
            : "Неизвестный пользователь";

        var canEdit = command.UserId == topic.AuthorId || command.IsAdmin || command.IsModerator;
        var canDelete = command.UserId == topic.AuthorId || command.IsAdmin;

        return new GetTopicWithCommentsResult(
            topic.Id,
            topic.Title,
            topic.AuthorId,
            topicAuthorNickname,
            topic.CreatedAt,
            commentResults,
            commentsPage.TotalCount,
            canEdit,
            canDelete);
    }
}
