using Enjoy.Application.Abstractions.Clock;
using Enjoy.Domain.Shared.Messaging;
using Enjoy.Infrastructure.BackgroundJobs.Options;
using Enjoy.Persistence.Constants;
using Enjoy.Persistence.Contexts;
using Enjoy.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Enjoy.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob : IJob
{
    private const string SqlBatchParameterPlaceholder = "{0}";

    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
    };

    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly OutboxProcessorOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        ApplicationDbContext dbContext,
        IPublisher publisher,
        IOptions<OutboxProcessorOptions> options,
        IDateTimeProvider dateTimeProvider,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _options = options.Value;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        int batchSize = Math.Max(1, _options.BatchSize);

        _logger.LogInformation("Starting outbox message processing (max batch {BatchSize})", batchSize);

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        List<OutboxMessage> messages = await GetOutboxMessagesAsync(batchSize, cancellationToken);

        foreach (OutboxMessage outboxMessage in messages)
        {
            Exception? exception = await ProcessOutboxMessageAsync(outboxMessage, cancellationToken);
            UpdateOutboxMessage(outboxMessage, exception);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Outbox processing completed ({Count} messages in this batch).",
            messages.Count);
    }

    private async Task<List<OutboxMessage>> GetOutboxMessagesAsync(
        int batchSize,
        CancellationToken cancellationToken)
    {
        string outboxTable = $"{Schemas.Application}.{TableNames.OutboxMessages}";
        string sql =
            $"""
            SELECT id, type, content, occurred_on_utc, processed_on_utc, error
            FROM {outboxTable}
            WHERE processed_on_utc IS NULL
            ORDER BY occurred_on_utc
            LIMIT {SqlBatchParameterPlaceholder}
            FOR UPDATE SKIP LOCKED
            """;

        return await _dbContext
            .Set<OutboxMessage>()
            .FromSqlRaw(sql, batchSize)
            .ToListAsync(cancellationToken);
    }

    private async Task<Exception?> ProcessOutboxMessageAsync(
        OutboxMessage outboxMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            IDomainEvent? domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                outboxMessage.Content,
                JsonSerializerSettings);

            if (domainEvent is null)
            {
                return new InvalidOperationException(
                    "Outbox message deserialization returned null.");
            }

            await _publisher.Publish(domainEvent, cancellationToken);
            return null;
        }
        catch (Exception caughtException)
        {
            _logger.LogError(
                caughtException,
                "Error processing outbox message {MessageId}",
                outboxMessage.Id);

            return caughtException;
        }
    }

    private void UpdateOutboxMessage(OutboxMessage outboxMessage, Exception? exception)
    {
        outboxMessage.ProcessedOnUtc = _dateTimeProvider.UtcNow;
        outboxMessage.Error = exception is null ? null : TruncateError(exception);
    }

    private static string TruncateError(Exception exception)
    {
        const int maxLen = 2000;
        string text = exception.ToString();
        return text.Length <= maxLen ? text : text[..maxLen];
    }
}
