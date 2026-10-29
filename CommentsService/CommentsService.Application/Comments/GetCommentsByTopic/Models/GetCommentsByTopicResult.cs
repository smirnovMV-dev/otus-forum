using System.Collections.Generic;

namespace CommentsService.Application.Comments.GetCommentsByTopic.Models;

public sealed record GetCommentsByTopicResult(
    IReadOnlyCollection<CommentTreeDto> Comments,
    int TotalCount);

public sealed record CommentTreeDto(
    long Id,
    long TopicId,
    long? ParentCommentId,
    long AuthorId,
    string Content,
    string CreatedAt,
    string UpdatedAt,
    IReadOnlyCollection<CommentTreeDto> Replies);
