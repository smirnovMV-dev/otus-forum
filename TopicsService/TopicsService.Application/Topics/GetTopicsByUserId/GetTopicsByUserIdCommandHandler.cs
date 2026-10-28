using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.UsersApiService;
using TopicsService.Application.Topics.GetTopicsByUserId.Models;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.GetTopicsByUserId;

internal sealed class GetTopicsByUserIdCommandHandler
    : IRequestHandler<GetTopicsByUserIdCommand, GetTopicsByUserIdResult>
{
    private readonly ITopicsRepository _topicsRepository;
    private readonly IUsersApiServiceAdapter _usersApiServiceAdapter;

    public GetTopicsByUserIdCommandHandler(
        ITopicsRepository topicsRepository,
        IUsersApiServiceAdapter usersApiServiceAdapter)
    {
        _topicsRepository = topicsRepository;
        _usersApiServiceAdapter = usersApiServiceAdapter;
    }

    public async Task<GetTopicsByUserIdResult> Handle(
        GetTopicsByUserIdCommand command,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _topicsRepository.GetTopicsByUserIdAsync(
            command.UserId,
            command.Page,
            command.PageSize,
            cancellationToken);

        var authorIds = pagedResult.Topics.Select(t => t.AuthorId).ToHashSet();

        var authorsNicknames = await _usersApiServiceAdapter.GetUsersNicknamesAsync(
            authorIds,
            cancellationToken);

        var topics = pagedResult.Topics.Select(topic =>
        {
            var authorNickname = authorsNicknames.TryGetValue(topic.AuthorId, out var nick)
                ? nick.Nickname
                : "Неизвестный пользователь";

            return new GetTopicsByUserIdItem(
                topic.Id,
                topic.Title,
                authorNickname,
                topic.CreatedAt);
        });

        return new GetTopicsByUserIdResult(
            [.. topics],
            pagedResult.TotalCount);
    }
}
