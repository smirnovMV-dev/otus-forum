using System;
using System.Collections.Generic;

namespace CommentsService.Application.Comments.GetCommentsByUserId.Models;

public sealed record GetCommentsByUserIdResult(
    IReadOnlyCollection<GetCommentsByUserIdItem> Comments,
    int TotalCount);

public sealed record GetCommentsByUserIdItem(
    long Id,
    long TopicId,
    long? ParentCommentId,
    string AuthorNickname,
    string Content,
    DateTimeOffset CreatedAt);
