using System;
using System.Collections.Generic;

namespace TopicsService.Application.Topics.GetTopicWithComments.Models;

public sealed record GetTopicWithCommentsResult(
    long TopicId,
    string Title,
    long AuthorId,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<CommentResult> Comments);

public sealed record CommentResult(
    long Id,
    long TopicId,
    long? ParentCommentId,
    string AuthorNickname,
    string Content,
    DateTimeOffset CreatedAt);
