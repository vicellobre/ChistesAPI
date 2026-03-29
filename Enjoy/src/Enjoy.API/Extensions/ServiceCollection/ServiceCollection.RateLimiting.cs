using System.Globalization;
using System.Threading.RateLimiting;
using Enjoy.API.Configurations;
using Enjoy.Infrastructure.Auth;
using Enjoy.Presentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Enjoy.API.Extensions.ServiceCollection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(RateLimitingOptions.SectionName);
        services.Configure<RateLimitingOptions>(section);

        RateLimitingOptions boundAtStartup = section.Get<RateLimitingOptions>() ?? new RateLimitingOptions();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = boundAtStartup.RejectionStatusCode;

            options.OnRejected = async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    RateLimitingOptions rateOptions = context.HttpContext.RequestServices
                        .GetRequiredService<IOptionsSnapshot<RateLimitingOptions>>().Value;

                    context.HttpContext.Response.Headers.RetryAfter = $"{retryAfter.TotalSeconds}";

                    ProblemDetailsFactory problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();
                    ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(
                        context.HttpContext,
                        rateOptions.RejectionStatusCode,
                        rateOptions.ProblemTitle,
                        detail: string.Format(
                            CultureInfo.InvariantCulture,
                            rateOptions.ProblemDetailFormat,
                            retryAfter.TotalSeconds));

                    await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: token);
                }
            };

            options.AddPolicy(RateLimitingPolicyNames.Default, httpContext =>
            {
                RateLimitingOptions rateOptions = httpContext.RequestServices
                    .GetRequiredService<IOptionsSnapshot<RateLimitingOptions>>().Value;

                string identityId = httpContext.User.GetIdentityId() ?? string.Empty;

                if (!string.IsNullOrEmpty(identityId))
                {
                    TokenBucketAuthenticatedOptions authOptions = rateOptions.Authenticated;
                    return RateLimitPartition.GetTokenBucketLimiter(
                        identityId,
                        _ =>
                            new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = authOptions.TokenLimit,
                                QueueProcessingOrder = authOptions.QueueProcessingOrder,
                                QueueLimit = authOptions.QueueLimit,
                                ReplenishmentPeriod = TimeSpan.FromMinutes(authOptions.ReplenishmentPeriodMinutes),
                                TokensPerPeriod = authOptions.TokensPerPeriod
                            });
                }

                FixedWindowAnonymousOptions anonymousOptions = rateOptions.Anonymous;
                return RateLimitPartition.GetFixedWindowLimiter(
                    anonymousOptions.PartitionKey,
                    _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = anonymousOptions.PermitLimit,
                            Window = TimeSpan.FromMinutes(anonymousOptions.WindowMinutes)
                        });
            });
        });

        return services;
    }
}
