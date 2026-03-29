using System;
using System.Diagnostics;
using Enjoy.Domain.Shared.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Enjoy.Application.Abstractions.Behaviours;

public sealed class RequestLoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : MediatR.IRequest<TResponse>
    where TResponse : IResult
{

    private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        try
        {
            _logger.LogInformation("Processing request {RequestName}", requestName);

            var stopwatch = Stopwatch.StartNew();
            TResponse result = await next(cancellationToken);
            stopwatch.Stop();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Request {RequestName} completed successfully", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Errors, true))
                {
                    _logger.LogError("Request {RequestName} completed with error", requestName);
                }
            }

            return result;
        }
        catch (Exception exception)
        {

            _logger.LogError(exception, "An error occurred while processing request {RequestName}", requestName);
            throw;
        }
        
    }
}
