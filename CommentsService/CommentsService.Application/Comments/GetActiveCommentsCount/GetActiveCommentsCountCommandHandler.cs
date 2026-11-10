using CommentsService.Application.Comments.GetActiveCommentsCount.Models;
using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.GetActiveCommentsCount;

internal sealed class GetActiveCommentsCountCommandHandler
    : IRequestHandler<GetActiveCommentsCountCommand, GetActiveCommentsCountResult>
{
    private readonly ICommentRepository _commentRepository;

    public GetActiveCommentsCountCommandHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<GetActiveCommentsCountResult> Handle(
        GetActiveCommentsCountCommand command,
        CancellationToken cancellationToken)
    {
        var topicCounts = await _commentRepository.CountByTopicIdsAsync(
            command.TopicIds,
            cancellationToken);

        return new GetActiveCommentsCountResult(topicCounts);
    }
}
