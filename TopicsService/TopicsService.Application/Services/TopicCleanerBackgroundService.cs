using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Topics.TopicCleaner;

namespace TopicsService.Application.Services;

public sealed class TopicCleanerBackgroundService(
    IOptions<TopicCleanerSettings> options,
    IServiceScopeFactory scopeFactory,
    ILogger<TopicCleanerBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = options.Value.CleanupInterval;

        logger.LogInformation(
            "Фоновый сервис очистки тем запущен. Интервал: {Interval}",
            interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync(stoppingToken);

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        logger.LogInformation("Фоновый сервис очистки тем останавливается.");
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var settings = options.Value;
            var cutoffDate = DateTimeOffset.UtcNow - settings.RetentionPeriod;

            logger.LogInformation(
                "Очистка тем началась. Дата отсечки: {CutoffDate}, Размер батча: {BatchSize}",
                cutoffDate,
                settings.BatchSize);

            var deletedCount = await mediator.Send(
                new DeleteSoftDeletedTopicsCommand(cutoffDate, settings.BatchSize),
                cancellationToken);

            logger.LogInformation(
                "Очистка тем завершена. Удалено тем: {DeletedCount}",
                deletedCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при очистке тем.");
        }
    }
}
