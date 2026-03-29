using Enjoy.Application.Auth.Commands.ExternalLoginCallback;
using Enjoy.Application.Auth.Commands.Login;
using Enjoy.Application.Auth.Commands.Register;
using Enjoy.Application.Auth.Queries.ExternalLogin;
using Enjoy.Domain.Shared.Results;
using Enjoy.Presentation.Abstractions;
using Enjoy.Presentation.Auth.V1.Models.ExternalLoginCallback;
using Enjoy.Presentation.Auth.V1.Models.GetExternalLogin;
using Enjoy.Presentation.Auth.V1.Models.Login;
using Enjoy.Presentation.Auth.V1.Models.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Enjoy.Presentation.Auth.V1.Controllers;

/// <summary>JWT authentication: login, registration, and external (OAuth) sign-in.</summary>
/// <remarks>
/// <para>Routes under <c>/api/auth</c>. They do not require Bearer unless the client sends it.</para>
/// <para><b>OAuth flow</b>: GET <c>external/{provider}-login</c> → open <c>redirectUrl</c> → provider redirects to <c>/api/auth/external/callback</c> with <c>code</c> and <c>state</c>.</para>
/// </remarks>
[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController(
    ILogger<AuthController> logger,
    ISender sender) : ApiController(sender, logger)
{
    /// <summary>Sign in with email and password.</summary>
    /// <param name="request">Credentials as JSON (<c>application/json</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>accessToken</c> + <c>refreshToken</c> (JWT).</returns>
    /// <remarks>
    /// <para><b>Request</b> — <c>POST /api/auth/login</c></para>
    /// <code language="json">
    /// {
    ///   "email": "user@localhost.dev",
    ///   "password": "User12345!"
    /// }
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "accessToken": "eyJhbGciOi...",
    ///   "refreshToken": "CfDJ8..."
    /// }
    /// </code>
    /// <para><b>Errors</b>: 400 (validation / ProblemDetails), 401 (invalid credentials).</para>
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var command = request.ToCommand();

        var result = await Sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<LoginResponse>(result));
    }

    /// <summary>Registers a user in Identity and in the domain.</summary>
    /// <param name="request">Name, email, password, and confirmation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>userId</c> and JWT tokens.</returns>
    /// <remarks>
    /// <para><b>Request</b> — <c>POST /api/auth/register</c></para>
    /// <code language="json">
    /// {
    ///   "name": "Jane Demo",
    ///   "email": "newuser@localhost.dev",
    ///   "password": "User12345!",
    ///   "confirmPassword": "User12345!"
    /// }
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "userId": "usr_019d...",
    ///   "accessToken": "eyJ...",
    ///   "refreshToken": "..."
    /// }
    /// </code>
    /// <para><b>Errors</b>: 400 (duplicate email, password mismatch, Identity policy, etc.).</para>
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponse>> RegisterAsync(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var command = request.ToCommand();

        var result = await Sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<RegisterResponse>(result));
    }

    /// <summary>Gets the external provider authorization URL (e.g. GitHub).</summary>
    /// <param name="provider">Path segment before <c>-login</c> (e.g. <c>github</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Object with absolute <c>redirectUrl</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b> — no body:</para>
    /// <code>
    /// GET /api/auth/external/github-login
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "redirectUrl": "https://github.com/login/oauth/authorize?client_id=...&amp;redirect_uri=...&amp;scope=...&amp;state=..."
    /// }
    /// </code>
    /// </remarks>
    [HttpGet("external/{provider}-login")]
    [ProducesResponseType<GetExternalLoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetExternalLoginResponse>> GetExternalLoginAsync(
        [FromRoute(Name = "provider")] string provider,
        CancellationToken cancellationToken)
    {
        var query = new GetExternalLoginQuery(provider);

        var result = await Sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<GetExternalLoginResponse>(result));
    }

    /// <summary>OAuth callback: exchanges the code for a session and tokens.</summary>
    /// <param name="request">Query <c>code</c>, <c>state</c>, and <c>provider</c>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>userId</c> and JWT tokens.</returns>
    /// <remarks>
    /// <para><b>Request</b> — browser arrives via provider redirect; example query:</para>
    /// <code>
    /// GET /api/auth/external/callback?code=abc123&amp;state=7f3a...&amp;provider=GitHub
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "userId": "usr_019d...",
    ///   "accessToken": "eyJ...",
    ///   "refreshToken": "..."
    /// }
    /// </code>
    /// <para><b>Errors</b>: 400, 401 (code exchange or user data failed).</para>
    /// </remarks>
    [HttpGet("external/callback")]
    [ProducesResponseType<ExternalLoginCallbackResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ExternalLoginCallbackResponse>> ExternalLoginCallbackAsync(
        [FromQuery] ExternalLoginCallbackRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var command = request.ToCommand();

        var result = await Sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<ExternalLoginCallbackResponse>(result));
    }
}
