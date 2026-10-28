using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

    public async Task<LatestTopicsPagedResult> GetLatestTopicsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (pageSize <= 0)
        {
            return new LatestTopicsPagedResult([], 0);
        }

        try
        {
            var totalCount = await _context.Topics
                .CountAsync(t => !t.IsDeleted, cancellationToken);

            var results = await _context.Topics
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new LatestTopicsDto(
                    t.Id,
                    t.Title,
                    t.AuthorId,
                    t.CreatedAt))
                .ToListAsync(cancellationToken);

            return new LatestTopicsPagedResult([.. results], totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка тем");
            return new LatestTopicsPagedResult([], 0);
        }
    }

    public async Task<Topic?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Topics.FindAsync([id], cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении темы с ID {TopicId}", id);
            return null;
        }
    }

    public async Task UpdateAsync(
        Topic topic,
        CancellationToken cancellationToken)
    {
        try
        {
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении темы с ID {TopicId}", topic.Id);
            throw;
        }
    }

    public async Task<int> DeleteBatchAsync(
        DateTimeOffset cutoffDate,
        int batchSize,
        CancellationToken cancellationToken)
    {
        try
        {
            var deletedCount = await _context.Topics
                .Where(t => t.IsDeleted && t.DeletedAt.HasValue && t.DeletedAt.Value < cutoffDate)
                .OrderBy(t => t.DeletedAt)
                .Take(batchSize)
                .ExecuteDeleteAsync(cancellationToken);

            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при пакетном удалении тем до {CutoffDate}", cutoffDate);
            throw;
        }
    }
}
