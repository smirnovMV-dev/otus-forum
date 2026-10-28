using System;
using System.Collections.Generic;

namespace CommentsService.API.Models.Comments.GetCommentsByUserId;

public sealed class GetCommentsByUserIdResponse
{
    public required IReadOnlyCollection<GetCommentsByUserIdItemResponse> Comments { get; set; }
    public required int TotalCount { get; set; }
}

public sealed class GetCommentsByUserIdItemResponse
{
    public required long Id { get; set; }
    public required long TopicId { get; set; }
    public required long? ParentCommentId { get; set; }
    public required string AuthorNickname { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
