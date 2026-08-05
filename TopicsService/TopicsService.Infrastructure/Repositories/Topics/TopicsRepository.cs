using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Domain.Entities;
using TopicsService.Infrastructure.Data;
using TopicsService.Infrastructure.Repositories.Topics.Models;

namespace TopicsService.Infrastructure.Repositories.Topics;

internal sealed class TopicsRepository : ITopicsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TopicsRepository> _logger;

    public TopicsRepository(
        ApplicationDbContext context,
        ILogger<TopicsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }    

    public async Task<int> CreateAsync(
        Topic topic,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.Topics.AddAsync(topic, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

    public async Task<IReadOnlyCollection<LatestTopicsModel>> GetLatestTopicsAsync(
        int count,
        CancellationToken cancellationToken)
    {
        if (count == 0)
        {
            return [];
        }

        try
        {
            var results = await _context.Topics
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .Select(t => new LatestTopicsDto(
                t.Id,
                t.Title,
                t.AuthorId,
                t.CreatedAt))
            .ToListAsync(cancellationToken);

            return [.. results.Select(LatestTopicsModel.Create)];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return [];
        }
    }
}
