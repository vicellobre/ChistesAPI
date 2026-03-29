using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;

namespace Enjoy.API.Configurations;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public int RejectionStatusCode { get; init; } = StatusCodes.Status429TooManyRequests;

    public string ProblemTitle { get; init; } = "Too Many Requests";

    public string ProblemDetailFormat { get; init; } =
        "Too many requests. Please try again after {0} seconds.";

    public TokenBucketAuthenticatedOptions Authenticated { get; init; } = new();

    public FixedWindowAnonymousOptions Anonymous { get; init; } = new();
}

public sealed class TokenBucketAuthenticatedOptions
{
    public int TokenLimit { get; init; } = 100;

    public int TokensPerPeriod { get; init; } = 25;

    public int QueueLimit { get; init; } = 5;

    public int ReplenishmentPeriodMinutes { get; init; } = 1;

    public QueueProcessingOrder QueueProcessingOrder { get; init; } = QueueProcessingOrder.OldestFirst;
}

public sealed class FixedWindowAnonymousOptions
{
    public string PartitionKey { get; init; } = "anonymous";

    public int PermitLimit { get; init; } = 5;

    public int WindowMinutes { get; init; } = 1;
}
