using CommentsService.Application.Comments.GetCommentsByUserId.Models;
using CommentsService.Application.Services.Externals.UsersApiService;
using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.GetCommentsByUserId;

internal sealed class GetCommentsByUserIdCommandHandler
    : IRequestHandler<GetCommentsByUserIdCommand, GetCommentsByUserIdResult>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUsersApiServiceAdapter _usersApiServiceAdapter;

    public GetCommentsByUserIdCommandHandler(
        ICommentRepository commentRepository,
        IUsersApiServiceAdapter usersApiServiceAdapter)
    {
        _commentRepository = commentRepository;
        _usersApiServiceAdapter = usersApiServiceAdapter;
    }

    public async Task<GetCommentsByUserIdResult> Handle(
        GetCommentsByUserIdCommand command,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _commentRepository.GetByUserIdAsync(
            command.UserId,
            command.Page,
            command.PageSize,
            cancellationToken);

        var authorIds = pagedResult.Comments.Select(c => c.AuthorId).Distinct().ToHashSet();
        var nicknames = await _usersApiServiceAdapter.GetUsersNicknamesAsync(
            authorIds,
            cancellationToken);

        var comments = pagedResult.Comments.Select(c => new GetCommentsByUserIdItem(
            c.Id,
            c.TopicId,
            c.ParentCommentId,
            nicknames.TryGetValue(c.AuthorId, out var nick) ? nick.Nickname : "Неизвестный пользователь",
            c.Content,
            c.CreatedAt));

        return new GetCommentsByUserIdResult([.. comments], pagedResult.TotalCount);
    }
}
