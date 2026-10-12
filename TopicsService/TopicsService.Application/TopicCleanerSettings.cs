using System;

namespace TopicsService.Application;

public sealed class TopicCleanerSettings
{
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(1);
    public int BatchSize { get; set; } = 100;
}
