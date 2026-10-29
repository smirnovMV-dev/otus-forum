using CommentsService.Application.Comments.GetCommentsByTopic.Models;
using MediatR;

namespace CommentsService.Application.Comments.GetCommentsByTopic;

public sealed record GetCommentsByTopicCommand(
    long TopicId,
    int Page,
    int PageSize) : IRequest<GetCommentsByTopicResult>;
