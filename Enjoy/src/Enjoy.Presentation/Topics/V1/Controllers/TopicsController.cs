using Enjoy.Application.Topics.Queries.ListTopics;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Presentation.Abstractions;
using Enjoy.Presentation.Topics.V1.Models.ListTopics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Enjoy.Presentation.Topics.V1.Controllers;

/// <summary>Topics for tagging or filtering jokes.</summary>
/// <remarks>
/// <para><c>GET /api/temas</c> with JWT. Useful for UI autocomplete.</para>
/// </remarks>
[Authorize(Roles = $"{Role.Admin},{Role.User}")]
[Route("api/temas")]
public sealed class TopicsController(
    ILogger<TopicsController> logger,
    ISender sender) : ApiController(sender, logger)
{
    /// <summary>Lists all available topics.</summary>
    /// <param name="request">Empty DTO.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>topics</c> array with <c>id</c> and <c>name</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// GET /api/temas
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "topics": [
    ///     {
    ///       "id": "top_...",
    ///       "name": "Programming"
    ///     }
    ///   ]
    /// }
    /// </code>
    /// </remarks>
    [HttpGet]
    [ProducesResponseType<ListTopicsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ListTopicsResponse>> ListTopicsAsync(
        [FromQuery] ListTopicsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = request.ToQuery();

        Result<ListTopicsQueryResponse> result = await Sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<ListTopicsResponse>(result));
    }
}
