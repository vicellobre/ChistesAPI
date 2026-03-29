using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Enjoy.Presentation.Middlewares;

public sealed class RequestContextLoggingMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public Task Invoke(HttpContext context)
    {
        string correlationId = GetCorrelationId(context);
        context.Items[RequestContextKeys.CorrelationIdItemKey] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            return next(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out StringValues value);
        return value.FirstOrDefault() is { Length: > 0 } id ? id : context.TraceIdentifier;
    }
}
