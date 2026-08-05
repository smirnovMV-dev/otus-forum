using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.UsersApiService;
using TopicsService.Application.Topics.GetLatestTopics.Models;
using TopicsService.Infrastructure.Repositories.Topics;

namespace TopicsService.Application.Topics.GetLatestTopics;

public sealed record GetLatestTopicsCommand(
    ) : IRequest<IReadOnlyCollection<GetLatestTopicsResult>>;

internal sealed class GetLatestTopicsCommandHandler
    : IRequestHandler<GetLatestTopicsCommand, IReadOnlyCollection<GetLatestTopicsResult>>
{
    private const int LatestTopicsCount = 5;

    private readonly ITopicsRepository _topicsRepository;
    private readonly IUsersApiServiceAdapter _usersApiServiceAdapter;

    public GetLatestTopicsCommandHandler(
        ITopicsRepository topicsRepository,
        IUsersApiServiceAdapter usersApiServiceAdapter)
    {
        _topicsRepository = topicsRepository;
        _usersApiServiceAdapter = usersApiServiceAdapter;
    }

    public async Task<IReadOnlyCollection<GetLatestTopicsResult>> Handle(
        GetLatestTopicsCommand command, 
        CancellationToken cancellationToken)
    {
        var topics = await _topicsRepository.GetLatestTopicsAsync(
            LatestTopicsCount,
            cancellationToken);

        var authorIds = topics.Select(t => t.AuthorId).ToHashSet();

        var authorsNicknames = await _usersApiServiceAdapter.GetUsersNicknamesAsync(
            authorIds,
            cancellationToken);

        return [.. topics.Select(topic =>
        {
            var authorNickname = authorsNicknames.TryGetValue(topic.AuthorId, out var authorNicknameVal)
            ? authorNicknameVal.Nickname
            : "Неизвестный пользователь";

            return new GetLatestTopicsResult(
                topic.Id,
                topic.Title,
                authorNickname,
                topic.CreatedAt);
        })];
    }
}
