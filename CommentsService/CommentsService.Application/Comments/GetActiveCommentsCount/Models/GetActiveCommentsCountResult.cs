using System.Collections.Generic;

namespace CommentsService.Application.Comments.GetActiveCommentsCount.Models;

public sealed record GetActiveCommentsCountResult(
    IReadOnlyDictionary<long, int> TopicCounts);
