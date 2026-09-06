using System;

namespace TopicsService.Application.Services.Externals.CommentsApiService.Models;

public sealed record CommentDto(
    long Id,
    long TopicId,
    long? ParentCommentId,
    long AuthorId,
    string Content,
    DateTimeOffset CreatedAt);
