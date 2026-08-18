namespace CommentsService.API.Models.Comments.CreateComment;

public sealed class CreateCommentRequest
{
    public required long TopicId { get; set; }
    public long? ParentCommentId { get; set; }
    public required long AuthorId { get; set; }
    public required string Content { get; set; }
}
