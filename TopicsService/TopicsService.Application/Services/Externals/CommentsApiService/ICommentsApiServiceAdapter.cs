using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.CommentsApiService.Models;

namespace TopicsService.Application.Services.Externals.CommentsApiService;

internal interface ICommentsApiServiceAdapter
{
    Task<IReadOnlyCollection<CommentDto>> GetCommentsByTopicIdAsync(
        long topicId,
        CancellationToken cancellationToken);
}
