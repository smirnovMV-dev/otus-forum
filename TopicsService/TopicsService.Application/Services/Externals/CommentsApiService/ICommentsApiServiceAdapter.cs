using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.CommentsApiService.Models;

namespace TopicsService.Application.Services.Externals.CommentsApiService;

internal interface ICommentsApiServiceAdapter
{
    Task<PagedCommentsDto> GetCommentsByTopicIdAsync(
        long topicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<PagedCommentsDto> GetRepliesAsync(
        long parentCommentId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, int>> GetActiveCommentsCountAsync(
        IReadOnlyCollection<long> topicIds,
        CancellationToken cancellationToken);
}
