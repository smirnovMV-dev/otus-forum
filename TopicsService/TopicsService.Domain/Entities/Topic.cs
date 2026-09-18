using System;

namespace TopicsService.Domain.Entities;

public sealed record Topic
{
    public long Id { get; private set; }

    public string Title { get; private set; }

    public long AuthorId { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public long SetId(long id) => Id = id;

    public static Topic Create(
        string title,
        long authorId,
        DateTimeOffset createdAt)
    => new(title,
        authorId,
        createdAt,
        createdAt);


    private Topic(
        string title,
        long authorId,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt) 
    {
        Title = title;
        AuthorId = authorId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void UpdateTitle(string newTitle, DateTimeOffset updatedAt)
    {
        Title = newTitle;
        UpdatedAt = updatedAt;
    }
}
