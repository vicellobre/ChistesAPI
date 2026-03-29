using System.Diagnostics;
using Enjoy.Presentation.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddProblemDetailsConfiguration(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                HttpContext httpContext = context.HttpContext;

                context.ProblemDetails.Instance =
                    $"{httpContext.Request.Method} {httpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);

                if (httpContext.Items.TryGetValue(RequestContextKeys.CorrelationIdItemKey, out object? cid) &&
                    cid is string correlationId)
                {
                    context.ProblemDetails.Extensions.TryAdd("correlationId", correlationId);
                }
                else
                {
                    context.ProblemDetails.Extensions.TryAdd("correlationId", httpContext.TraceIdentifier);
                }

                Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity ?? Activity.Current;
                if (activity is not null)
                {
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity.TraceId.ToHexString());
                }
            };
        });

        return services;
    }

    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }
}
