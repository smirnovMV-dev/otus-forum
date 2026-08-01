namespace TopicsService.Domain.Entities;

public sealed record Topic
{
    public long Id { get; private set; }

    public string Caption { get; }

    public long AuthorId { get; }

    public DateTimeOffset CreatedAt { get; }

    public long SetId(long id) => Id = id;

    public static Topic Create(
        string caption,
        long authorId,
        DateTimeOffset createdAt)
    => new(caption,
        authorId,
        createdAt);


    private Topic(
        string caption,
        long authorId,
        DateTimeOffset createdAt) 
    {
        Caption = caption;
        AuthorId = authorId;
        CreatedAt = createdAt;
    }
}
