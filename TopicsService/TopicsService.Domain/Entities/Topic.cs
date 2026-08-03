namespace TopicsService.Domain.Entities;

public sealed record Topic
{
    public long Id { get; private set; }

    public string Title { get; }

    public long AuthorId { get; }

    public DateTimeOffset CreatedAt { get; }

    public long SetId(long id) => Id = id;

    public static Topic Create(
        string title,
        long authorId,
        DateTimeOffset createdAt)
    => new(title,
        authorId,
        createdAt);


    private Topic(
        string title,
        long authorId,
        DateTimeOffset createdAt) 
    {
        Title = title;
        AuthorId = authorId;
        CreatedAt = createdAt;
    }
}
