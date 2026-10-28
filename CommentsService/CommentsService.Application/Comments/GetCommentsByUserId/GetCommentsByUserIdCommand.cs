using MediatR;
using CommentsService.Application.Comments.GetCommentsByUserId.Models;

namespace CommentsService.Application.Comments.GetCommentsByUserId;

public sealed record GetCommentsByUserIdCommand(
    long UserId,
    int Page,
    int PageSize) : IRequest<GetCommentsByUserIdResult>;
