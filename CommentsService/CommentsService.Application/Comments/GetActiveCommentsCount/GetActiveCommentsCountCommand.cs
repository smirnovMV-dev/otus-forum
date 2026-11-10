using CommentsService.Application.Comments.GetActiveCommentsCount.Models;
using MediatR;
using System.Collections.Generic;

namespace CommentsService.Application.Comments.GetActiveCommentsCount;

public sealed record GetActiveCommentsCountCommand(
    IReadOnlyCollection<long> TopicIds) : IRequest<GetActiveCommentsCountResult>;
