using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Domain.Entities;
using TopicsService.Infrastructure.Data;

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
}
