using System;

namespace CommentsService.Domain.Entities;

public sealed record Comment
{
    public long Id { get; private set; }
    public long TopicId { get; }
    public long? ParentCommentId { get; }
    public long AuthorId { get; }
    public string Content { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }

    public static Comment Create(
        long topicId,
        long? parentCommentId,
        long authorId,
        string content,
        DateTimeOffset createdAt)
    {
        return new Comment(
            topicId,
            parentCommentId,
            authorId,
            content,
            createdAt);
    }

    public long SetId(long id) => Id = id;

    private Comment (
        long topicId,
        long? parentCommentId,
        long authorId,
        string content,
        DateTimeOffset createdAt)
    {
        TopicId = topicId;
        ParentCommentId = parentCommentId;
        AuthorId = authorId;
        Content = content;        
        CreatedAt = createdAt;
    }
}
