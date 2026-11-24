using MediatR;
using TopicsService.Application.Comments.GetCommentReplies.Models;

namespace TopicsService.Application.Comments.GetCommentReplies;

public sealed record GetCommentRepliesCommand(
    long ParentCommentId,
    long UserId,
    bool IsAdmin,
    bool IsModerator,
    int Page,
    int PageSize) : IRequest<GetCommentRepliesResult>;
