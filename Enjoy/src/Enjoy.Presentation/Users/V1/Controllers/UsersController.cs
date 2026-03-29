using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Users.Queries.GetCurrentUser;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Presentation.Abstractions;
using Enjoy.Presentation.Users.V1.Models.GetAdmin;
using Enjoy.Presentation.Users.V1.Models.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Enjoy.Presentation.Users.V1.Controllers;

/// <summary>Authenticated user profile and Admin-only demo endpoint.</summary>
/// <remarks>
/// <para><c>GET /api/usuario</c> — JWT with role <c>User</c> or <c>Admin</c>.</para>
/// <para><c>GET /api/admin</c> — <c>Admin</c> role only.</para>
/// </remarks>
[Route("api")]
public sealed class UsersController(
    ILogger<UsersController> logger,
    ISender sender,
    IUserContext userContext) : ApiController(sender, logger)
{
    /// <summary>Returns the current user from the JWT.</summary>
    /// <param name="request">Empty DTO (no required query).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>id</c>, <c>name</c>, <c>email</c>, <c>role</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// GET /api/usuario
    /// Authorization: Bearer &lt;access_token&gt;
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "id": "usr_019d...",
    ///   "name": "Demo",
    ///   "email": "user@localhost.dev",
    ///   "role": "User"
    /// }
    /// </code>
    /// </remarks>
    [Authorize(Roles = $"{Role.Admin},{Role.User}")]
    [HttpGet("usuario")]
    [ProducesResponseType<GetCurrentUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<GetCurrentUserResponse>> GetCurrentUserAsync(
        [FromQuery] GetCurrentUserRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetCurrentUserQuery> queryResult = request.ToQuery(userContext);

        if (queryResult.IsFailure)
        {
            return ProblemFromResult<GetCurrentUserResponse>(queryResult);
        }

        Result<GetCurrentUserQueryResponse> result = await Sender.Send(queryResult.Value, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<GetCurrentUserResponse>(result));
    }

    /// <summary>Demo: route only available with Admin role.</summary>
    /// <param name="request">Empty DTO.</param>
    /// <returns>Confirmation message.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// GET /api/admin
    /// Authorization: Bearer &lt;access_token_admin&gt;
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "message": "…"
    /// }
    /// </code>
    /// </remarks>
    [Authorize(Roles = Role.Admin)]
    [HttpGet("admin")]
    [ProducesResponseType<GetAdminResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public ActionResult<GetAdminResponse> GetAdminAsync([FromQuery] GetAdminRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = request.ToResponse();
        return Ok(response);
    }
}
