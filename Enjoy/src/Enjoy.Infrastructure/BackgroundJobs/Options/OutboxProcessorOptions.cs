namespace Enjoy.Infrastructure.BackgroundJobs.Options;

public sealed class OutboxProcessorOptions
{
    public const string SectionName = "OutboxProcessor";

    public int ProcessIntervalSeconds { get; init; } = 100;

    public int BatchSize { get; init; } = 20;
}
