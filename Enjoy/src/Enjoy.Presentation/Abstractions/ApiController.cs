using Asp.Versioning;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Presentation.Problems;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;

namespace Enjoy.Presentation.Abstractions;

/// <summary>Base API v1 controller: MediatR, rate limiting, and Problem Details helpers for domain results.</summary>
/// <remarks>
/// <para>Domain errors → RFC 7807 (<c>ProblemDetails</c>).</para>
/// <para><b>Sample error body</b></para>
/// <code language="json">
/// {
///   "type": "https://httpstatuses.io/400",
///   "title": "…",
///   "status": 400,
///   "detail": "…",
///   "traceId": "00-…"
/// }
/// </code>
/// <para>Anonymous routes without Bearer; otherwise: <c>Authorization: Bearer &lt;accessToken&gt;</c>.</para>
/// </remarks>
[ApiVersion(1.0)]
[ApiController]
[EnableRateLimiting(RateLimitingPolicyNames.Default)]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;
    protected readonly ILogger Logger;

    protected ApiController(ISender sender, ILogger logger)
    {
        Sender = sender;
        Logger = logger;
    }

    protected ActionResult<T> ProblemFromResult<T>(Result result)
    {
        var problem = result.ToProblemDetails();
        return StatusCode(problem.Status!.Value, problem);
    }

    protected ActionResult<T> ProblemFromError<T>(Error error)
    {
        var problem = error.ToProblemDetails();
        return StatusCode(problem.Status!.Value, problem);
    }

    protected ActionResult<TResponse> CreatedAtAction<TResponse>(
        string actionName,
        object routeValues,
        TResponse response)
    {
        var uri = Url.Action(actionName, routeValues);
        return Created(uri, response);
    }
}
