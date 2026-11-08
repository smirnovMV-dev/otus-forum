using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AuthUsersService.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthUsersService.Infrastructure.BackgroundServices;

/// <summary>
/// Периодически удаляет просроченные и давно отозванные refresh-токены.
/// </summary>
internal sealed class RefreshTokenCleaner : BackgroundService
{
    private static readonly TimeSpan DefaultCleanupInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan DefaultRetentionPeriod = TimeSpan.FromDays(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshTokenCleaner> _logger;
    private readonly TimeSpan _cleanupInterval;
    private readonly TimeSpan _retentionPeriod;

    public RefreshTokenCleaner(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<RefreshTokenCleaner> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _cleanupInterval = ReadTimeSpan(
            configuration["RefreshTokenCleaner:CleanupInterval"],
            DefaultCleanupInterval);
        _retentionPeriod = ReadTimeSpan(
            configuration["RefreshTokenCleaner:RetentionPeriod"],
            DefaultRetentionPeriod);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_cleanupInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await CleanupAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Штатная остановка хоста.
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();

            var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

            await refreshTokenRepository.DeleteExpiredAsync(
                DateTimeOffset.UtcNow - _retentionPeriod,
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка очистки просроченных refresh-токенов.");
        }
    }

    private static TimeSpan ReadTimeSpan(string? value, TimeSpan fallback)
    {
        return TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var result) && result > TimeSpan.Zero
            ? result
            : fallback;
    }
}
