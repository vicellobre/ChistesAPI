using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Jokes.Commands.CreateJoke;
using Enjoy.Application.Jokes.Commands.DeleteJoke;
using Enjoy.Application.Jokes.Commands.UpdateJoke;
using Enjoy.Application.Jokes.Queries.FilterJokes;
using Enjoy.Application.Jokes.Queries.GetCombinedJoke;
using Enjoy.Application.Jokes.Queries.GetPairedJokes;
using Enjoy.Application.Jokes.Queries.GetRandomJoke;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Presentation.Abstractions;
using Enjoy.Presentation.Common;
using Enjoy.Presentation.Jokes.V1.Models.CreateJoke;
using Enjoy.Presentation.Jokes.V1.Models.DeleteJoke;
using Enjoy.Presentation.Jokes.V1.Models.FilterJokes;
using Enjoy.Presentation.Jokes.V1.Models.GetCombinedJoke;
using Enjoy.Presentation.Jokes.V1.Models.GetPairedJokes;
using Enjoy.Presentation.Jokes.V1.Models.GetRandomJoke;
using Enjoy.Presentation.Jokes.V1.Models.UpdateJoke;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Enjoy.Presentation.Jokes.V1.Controllers;

/// <summary>Jokes: random, paired, combined queries, filtering, and CRUD.</summary>
/// <remarks>
/// <para>Requires <c>Authorization: Bearer &lt;JWT&gt;</c> and role <c>User</c> or <c>Admin</c>.</para>
/// <para>Base path: <c>/api/chistes</c>. Rate limiting may apply (429).</para>
/// </remarks>
[Authorize(Roles = $"{Role.Admin},{Role.User}")]
[Route("api/chistes")]
public sealed class JokesController(
    ILogger<JokesController> logger,
    ISender sender) : ApiController(sender, logger)
{
    /// <summary>Gets a random joke (Chuck or Dad by origin).</summary>
    /// <param name="request">Optional <c>origin</c> query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>text</c> and <c>origin</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// GET /api/chistes/aleatorio?origin=Chuck
    /// </code>
    /// <para><b>200 OK</b> (camelCase serialization)</para>
    /// <code language="json">
    /// {
    ///   "text": "…",
    ///   "origin": "Chuck"
    /// }
    /// </code>
    /// </remarks>
    [HttpGet("aleatorio")]
    [ProducesResponseType<GetRandomJokeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<GetRandomJokeResponse>> GetRandomJokeAsync(
        [FromQuery] GetRandomJokeRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = request.ToQuery();

        var result = await Sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<GetRandomJokeResponse>(result));
    }

    /// <summary>Returns several Chuck+Dad pairs fused with Gemini (one fusion per batch).</summary>
    /// <param name="request">Empty DTO (no required query).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>pairs</c> list with <c>chuck</c>, <c>dad</c>, <c>combined</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// GET /api/chistes/emparejados
    /// </code>
    /// <para><b>200 OK</b> (illustrative shape)</para>
    /// <code language="json">
    /// {
    ///   "pairs": [
    ///     {
    ///       "chuck": "…",
    ///       "dad": "…",
    ///       "combined": "…"
    ///     }
    ///   ]
    /// }
    /// </code>
    /// </remarks>
    [HttpGet("emparejados")]
    [ProducesResponseType<GetPairedJokesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<GetPairedJokesResponse>> GetPairedJokesAsync(
        [FromQuery] GetPairedJokesRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = request.ToQuery();

        var result = await Sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<GetPairedJokesResponse>(result));
    }

    /// <summary>Gets a combined joke (template + fragments from sources).</summary>
    /// <param name="request">No required query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>text</c> and <c>sourceFragments</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// GET /api/chistes/combinado
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "text": "…",
    ///   "sourceFragments": [
    ///     {
    ///       "source": "Chuck",
    ///       "fragment": "…"
    ///     }
    ///   ]
    /// }
    /// </code>
    /// </remarks>
    [HttpGet("combinado")]
    [ProducesResponseType<GetCombinedJokeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<GetCombinedJokeResponse>> GetCombinedJokeAsync(
        [FromQuery] GetCombinedJokeRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = request.ToQuery();

        var result = await Sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<GetCombinedJokeResponse>(result));
    }

    /// <summary>Creates a joke in the domain.</summary>
    /// <param name="request">JSON body with <c>text</c>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated <c>jokeId</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b> — <c>POST /api/chistes</c></para>
    /// <code language="json">
    /// {
    ///   "text": "Why is Oct 31 == Dec 25? Because 25 in octal is the same day as 31 in decimal."
    /// }
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "jokeId": "jke_01..."
    /// }
    /// </code>
    /// </remarks>
    [HttpPost]
    [ProducesResponseType<CreateJokeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CreateJokeResponse>> CreateJokeAsync(
        [FromBody] CreateJokeRequest request,
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
            onFailure: _ => ProblemFromResult<CreateJokeResponse>(result));
    }

    /// <summary>Lists or filters jokes by words, author, topic, etc.</summary>
    /// <param name="request">Optional query: <c>minWords</c>, <c>contains</c>, <c>authorId</c>, <c>topicId</c>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>jokes</c> collection with metadata.</returns>
    /// <remarks>
    /// <para><b>Request</b> (all filters optional)</para>
    /// <code>
    /// GET /api/chistes/filtrar?contains=developer&amp;minWords=5
    /// </code>
    /// </remarks>
    [HttpGet("filtrar")]
    [ProducesResponseType<FilterJokesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<FilterJokesResponse>> FilterJokesAsync(
        [FromQuery] FilterJokesRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = request.ToQuery();

        var result = await Sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: r =>
            {
                var response = r.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<FilterJokesResponse>(result));
    }

    /// <summary>Updates the text of an existing joke.</summary>
    /// <param name="request">Route <c>id</c> + body <c>newText</c>.</param>
    /// <param name="userContext">JWT user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>completed</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b></para>
    /// <code>
    /// PUT /api/chistes/jke_01abc
    /// </code>
    /// <para><b>Body</b></para>
    /// <code language="json">
    /// {
    ///   "newText": "Revised text."
    /// }
    /// </code>
    /// <para><b>Errors</b>: 404 (unknown id), 403 (forbidden).</para>
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType<OperationCompletedResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<OperationCompletedResponse>> UpdateJokeAsync(
        UpdateJokeRequest request,
        [FromServices] IUserContext userContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var commandResult = request.ToCommand(userContext);

        if (commandResult.IsFailure)
        {
            return ProblemFromResult<OperationCompletedResponse>(commandResult);
        }

        Result mediatrResult = await Sender.Send(commandResult.Value, cancellationToken);

        return mediatrResult.Match(
            onSuccess: () =>
            {
                var response = new OperationCompletedResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<OperationCompletedResponse>(mediatrResult));
    }

    /// <summary>Deletes a joke by id.</summary>
    /// <param name="request">Id in route.</param>
    /// <param name="userContext">JWT user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>completed</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b> — no body</para>
    /// <code>
    /// DELETE /api/chistes/jke_01abc
    /// </code>
    /// <para><b>Errors</b>: 404 if not found.</para>
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType<OperationCompletedResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<OperationCompletedResponse>> DeleteJokeAsync(
        DeleteJokeRequest request,
        [FromServices] IUserContext userContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var commandResult = request.ToCommand(userContext);

        if (commandResult.IsFailure)
        {
            return ProblemFromResult<OperationCompletedResponse>(commandResult);
        }

        Result mediatrResult = await Sender.Send(commandResult.Value, cancellationToken);

        return mediatrResult.Match(
            onSuccess: () =>
            {
                var response = new OperationCompletedResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<OperationCompletedResponse>(mediatrResult));
    }
}
