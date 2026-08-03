using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Topics.GetLatestTopics.Models;

namespace TopicsService.Application.Topics.GetLatestTopics;

public sealed record GetLatestTopicsCommand(
    ) : IRequest<IReadOnlyCollection<GetLatestTopicsResult>>;

internal sealed class GetLatestTopicsCommandHandler
    : IRequestHandler<GetLatestTopicsCommand, IReadOnlyCollection<GetLatestTopicsResult>>
{
    public async Task<IReadOnlyCollection<GetLatestTopicsResult>> Handle(
        GetLatestTopicsCommand command, 
        CancellationToken cancellationToken)
    {
        return [];
    }
}
