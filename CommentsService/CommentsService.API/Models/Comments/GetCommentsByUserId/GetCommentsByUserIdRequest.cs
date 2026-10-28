namespace CommentsService.API.Models.Comments.GetCommentsByUserId;

public sealed class GetCommentsByUserIdRequest
{
    public required long UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
