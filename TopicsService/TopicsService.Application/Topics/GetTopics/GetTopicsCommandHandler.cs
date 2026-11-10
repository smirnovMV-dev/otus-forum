using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.CommentsApiService;
using TopicsService.Application.Services.Externals.UsersApiService;
using TopicsService.Application.Topics.GetTopics.Models;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.GetTopics;

internal sealed class GetTopicsCommandHandler
    : IRequestHandler<GetTopicsCommand, GetTopicsResult>
{
    private readonly ITopicsRepository _topicsRepository;
    private readonly IUsersApiServiceAdapter _usersApiServiceAdapter;
    private readonly ICommentsApiServiceAdapter _commentsApiServiceAdapter;

    public GetTopicsCommandHandler(
        ITopicsRepository topicsRepository,
        IUsersApiServiceAdapter usersApiServiceAdapter,
        ICommentsApiServiceAdapter commentsApiServiceAdapter)
    {
        _topicsRepository = topicsRepository;
        _usersApiServiceAdapter = usersApiServiceAdapter;
        _commentsApiServiceAdapter = commentsApiServiceAdapter;
    }

    public async Task<GetTopicsResult> Handle(
        GetTopicsCommand command,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _topicsRepository.GetLatestTopicsAsync(
            command.Page,
            command.PageSize,
            cancellationToken);

        var authorIds = pagedResult.Topics.Select(t => t.AuthorId).ToHashSet();

        var authorsNicknames = await _usersApiServiceAdapter.GetUsersNicknamesAsync(
            authorIds,
            cancellationToken);

        var topicIds = pagedResult.Topics.Select(t => t.Id).ToList();

        var topicCounts = await _commentsApiServiceAdapter.GetActiveCommentsCountAsync(
            topicIds,
            cancellationToken);

        var topics = pagedResult.Topics.Select(topic =>
        {
            var authorNickname = authorsNicknames.TryGetValue(topic.AuthorId, out var authorNicknameVal)
                ? authorNicknameVal.Nickname
                : "Неизвестный пользователь";

            var commentsCount = topicCounts.TryGetValue(topic.Id, out var count) ? count : 0;

            return new GetTopicsItem(
                topic.Id,
                topic.Title,
                authorNickname,
                topic.CreatedAt,
                commentsCount);
        }).ToList();

        return new GetTopicsResult(
            [.. topics],
            pagedResult.TotalCount);
    }
}
