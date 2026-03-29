using Enjoy.Application.Maths.Queries.GetLeastCommonMultiple;
using Enjoy.Application.Maths.Queries.GetNextNumber;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Presentation.Abstractions;
using Enjoy.Presentation.Maths.V1.Models.GetLeastCommonMultiple;
using Enjoy.Presentation.Maths.V1.Models.GetNextNumber;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Enjoy.Presentation.Maths.V1.Controllers;

/// <summary>Numeric examples: LCM and next term in a sequence.</summary>
/// <remarks>
/// <para>Prefix <c>/api/matematicas</c>. Requires JWT (<c>User</c> or <c>Admin</c>).</para>
/// </remarks>
[Authorize(Roles = $"{Role.Admin},{Role.User}")]
[Route("api/matematicas")]
public sealed class MathsController(
    ILogger<MathsController> logger,
    ISender sender) : ApiController(sender, logger)
{
    /// <summary>Computes the least common multiple of a list of integers.</summary>
    /// <param name="request">Query <c>numbers</c> comma-separated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>leastCommonMultiple</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// GET /api/matematicas/mcm?numbers=4,6,8
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "leastCommonMultiple": 24
    /// }
    /// </code>
    /// </remarks>
    [HttpGet("mcm")]
    [ProducesResponseType<GetLeastCommonMultipleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<GetLeastCommonMultipleResponse>> GetLeastCommonMultipleAsync(
        [FromQuery] GetLeastCommonMultipleRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetLeastCommonMultipleQuery> queryResult = request.ToQuery();
        if (queryResult.IsFailure)
        {
            return ProblemFromResult<GetLeastCommonMultipleResponse>(queryResult);
        }

        Result<GetLeastCommonMultipleQueryResponse> result = await Sender.Send(queryResult.Value, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<GetLeastCommonMultipleResponse>(result));
    }

    /// <summary>Computes the next number in the sequence defined in the application.</summary>
    /// <param name="request">Query <c>number</c> (current integer).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>nextNumber</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// GET /api/matematicas/siguiente-numero?number=7
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "nextNumber": 8
    /// }
    /// </code>
    /// </remarks>
    [HttpGet("siguiente-numero")]
    [ProducesResponseType<GetNextNumberResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<GetNextNumberResponse>> GetNextNumberAsync(
        [FromQuery] GetNextNumberRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetNextNumberQuery> queryResult = request.ToQuery();
        if (queryResult.IsFailure)
        {
            return ProblemFromResult<GetNextNumberResponse>(queryResult);
        }

        Result<GetNextNumberQueryResponse> result = await Sender.Send(queryResult.Value, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<GetNextNumberResponse>(result));
    }
}
