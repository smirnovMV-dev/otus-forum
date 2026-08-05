using System;

namespace TopicsService.Infrastructure.Repositories.Topics.Models;

public sealed record LatestTopicsModel
{
    public long Id { get; }

    public string Title { get; }
    
    public long AuthorId { get; }
    
    public DateTimeOffset CreatedAt { get; }

    internal static LatestTopicsModel Create(LatestTopicsDto dto)
    {
        return new LatestTopicsModel(
            dto.Id,
            dto.Title,
            dto.AuthorId,
            dto.CreatedAt);
    }

    private LatestTopicsModel(
        long id,
        string title,
        long authorId,
        DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        AuthorId = authorId;
        CreatedAt = createdAt;
    }
}
