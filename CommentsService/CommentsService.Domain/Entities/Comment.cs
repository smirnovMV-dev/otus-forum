using System;

namespace CommentsService.Domain.Entities;

public sealed record Comment
{
    public long Id { get; private set; }
    public long TopicId { get; private set; }
    public long? ParentCommentId { get; private set; }
    public long AuthorId { get; }
    public string Content { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }

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

    public void Update(long topicId, long? parentCommentId, string content, DateTimeOffset updatedAt, long? updatedByUserId = null)
    {
        TopicId = topicId;
        ParentCommentId = parentCommentId;
        Content = content;
        UpdatedAt = updatedAt;
        UpdatedByUserId = updatedByUserId;
    }

    public void Update(string content, DateTimeOffset updatedAt, long? updatedByUserId = null)
    {
        Content = content;
        UpdatedAt = updatedAt;
        UpdatedByUserId = updatedByUserId;
    }

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
