using CommentsService.Application.Comments.GetRepliesByParentId.Models;
using MediatR;

namespace CommentsService.Application.Comments.GetRepliesByParentId;

public sealed record GetRepliesByParentIdCommand(
    long ParentCommentId,
    int Page,
    int PageSize) : IRequest<GetRepliesByParentIdResult>;
