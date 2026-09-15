namespace CommentsService.API.Models.Comments.UpdateComment;

public sealed class UpdateCommentRequest
{
    public required long TopicId { get; set; }
    public long? ParentCommentId { get; set; }
    public required string Content { get; set; }
}
