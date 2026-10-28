using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.UsersApiService;
using TopicsService.Application.Topics.GetLatestTopics.Models;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.GetLatestTopics;

public sealed record GetLatestTopicsCommand(
    int Page,
    int PageSize
) : IRequest<GetLatestTopicsResult>;

internal sealed class GetLatestTopicsCommandHandler
    : IRequestHandler<GetLatestTopicsCommand, GetLatestTopicsResult>
{
    private readonly ITopicsRepository _topicsRepository;
    private readonly IUsersApiServiceAdapter _usersApiServiceAdapter;

    public GetLatestTopicsCommandHandler(
        ITopicsRepository topicsRepository,
        IUsersApiServiceAdapter usersApiServiceAdapter)
    {
        _topicsRepository = topicsRepository;
        _usersApiServiceAdapter = usersApiServiceAdapter;
    }

    public async Task<GetLatestTopicsResult> Handle(
        GetLatestTopicsCommand command, 
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

        var topics = pagedResult.Topics.Select(topic =>
        {
            var authorNickname = authorsNicknames.TryGetValue(topic.AuthorId, out var authorNicknameVal)
                ? authorNicknameVal.Nickname
                : "Неизвестный пользователь";

            return new GetLatestTopicsItem(
                topic.Id,
                topic.Title,
                authorNickname,
                topic.CreatedAt);
        });

        return new GetLatestTopicsResult(
            [.. topics],
            pagedResult.TotalCount);
    }
}
