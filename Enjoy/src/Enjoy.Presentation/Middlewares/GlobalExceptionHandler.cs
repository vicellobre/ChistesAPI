using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Enjoy.Presentation.Middlewares;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private const string InternalErrorDetail = "An error occurred while processing the request.";

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var mapped = MapException(exception);

        LogException(exception, mapped);

        var problemDetails = new ProblemDetails
        {
            Title = mapped.Title,
            Type = $"https://httpstatuses.io/{mapped.StatusCode}",
            Detail = mapped.Detail,
            Status = mapped.StatusCode
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private void LogException(Exception exception, MappedException mapped)
    {
        switch (mapped.LogLevel)
        {
            case LogLevel.Error:
                logger.LogError(exception, "Unhandled error while processing the request.");
                break;
            case LogLevel.Debug:
                logger.LogDebug("Operation canceled or the client closed the connection.");
                break;
            default:
                logger.Log(
                    mapped.LogLevel,
                    exception,
                    "Exception mapped to HTTP {StatusCode}: {Message}",
                    mapped.StatusCode,
                    exception.Message);
                break;
        }
    }

    private static MappedException MapException(Exception exception)
    {
        return exception switch
        {
            OperationCanceledException or TaskCanceledException => new MappedException(
                StatusCodes.Status499ClientClosedRequest,
                "Request canceled",
                "The operation was canceled or the client closed the connection.",
                LogLevel.Debug),

            UnauthorizedAccessException => new MappedException(
                StatusCodes.Status403Forbidden,
                "Access denied",
                "You do not have permission to perform this operation.",
                LogLevel.Warning),

            KeyNotFoundException => new MappedException(
                StatusCodes.Status404NotFound,
                "Resource not found",
                string.IsNullOrWhiteSpace(exception.Message)
                    ? "The requested resource does not exist."
                    : exception.Message,
                LogLevel.Warning),

            ArgumentNullException or ArgumentException => new MappedException(
                StatusCodes.Status400BadRequest,
                "Bad request",
                exception.Message,
                LogLevel.Warning),

            _ => new MappedException(
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                InternalErrorDetail,
                LogLevel.Error)
        };
    }

    private readonly record struct MappedException(
        int StatusCode,
        string Title,
        string Detail,
        LogLevel LogLevel);
}
