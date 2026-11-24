using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Comments.GetCommentReplies.Models;
using TopicsService.Application.Services.Externals.CommentsApiService;
using TopicsService.Application.Services.Externals.UsersApiService;

namespace TopicsService.Application.Comments.GetCommentReplies;

internal sealed class GetCommentRepliesCommandHandler
    : IRequestHandler<GetCommentRepliesCommand, GetCommentRepliesResult>
{
    private readonly ICommentsApiServiceAdapter _commentsApiServiceAdapter;
    private readonly IUsersApiServiceAdapter _usersApiServiceAdapter;

    public GetCommentRepliesCommandHandler(
        ICommentsApiServiceAdapter commentsApiServiceAdapter,
        IUsersApiServiceAdapter usersApiServiceAdapter)
    {
        _commentsApiServiceAdapter = commentsApiServiceAdapter;
        _usersApiServiceAdapter = usersApiServiceAdapter;
    }

    public async Task<GetCommentRepliesResult> Handle(
        GetCommentRepliesCommand command,
        CancellationToken cancellationToken)
    {
        var page = await _commentsApiServiceAdapter.GetRepliesAsync(
            command.ParentCommentId,
            command.Page,
            command.PageSize,
            cancellationToken);

        if (page.Comments.Count == 0)
        {
            return new GetCommentRepliesResult([], page.TotalCount);
        }

        var authorIds = page.Comments
            .Select(c => c.AuthorId)
            .ToHashSet();

        var nicknames = await _usersApiServiceAdapter.GetUsersNicknamesAsync(authorIds, cancellationToken);

        var replies = page.Comments
            .Select(c => CommentResultMapper.Map(c, nicknames, command.UserId, command.IsAdmin, command.IsModerator))
            .ToList();

        return new GetCommentRepliesResult(replies, page.TotalCount);
    }
}
