using System;
using System.Collections.Generic;

namespace TopicsService.Application.Services.Externals.CommentsApiService.Models;

public sealed record CommentDto(
    long Id,
    long TopicId,
    long? ParentCommentId,
    long AuthorId,
    string Content,
    string CreatedAt,
    string UpdatedAt,
    IReadOnlyCollection<CommentDto> Replies);
