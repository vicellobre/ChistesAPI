using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Presentation.Abstractions;
using Enjoy.Presentation.Notifications.V1.Models.SendNotification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Enjoy.Presentation.Notifications.V1.Controllers;

/// <summary>Sending notifications (demo; administrators only).</summary>
/// <remarks>
/// <para>The body uses Spanish JSON names: <c>destinatario</c>, <c>mensaje</c>, <c>tipoNotificacion</c> (<c>JsonPropertyName</c>).</para>
/// </remarks>
[Authorize(Roles = Role.Admin)]
[Route("api/notificaciones")]
public sealed class NotificationsController(
    ILogger<NotificationsController> logger,
    ISender sender) : ApiController(sender, logger)
{
    /// <summary>Processes a notification send from the payload.</summary>
    /// <param name="request">Recipient, message, and type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Echo with <c>recipient</c>, <c>notificationType</c>, <c>sent</c>.</returns>
    /// <remarks>
    /// <para><b>Request</b> — <c>POST /api/notificaciones/enviar</c></para>
    /// <code language="json">
    /// {
    ///   "destinatario": "admin@localhost.dev",
    ///   "mensaje": "Test message",
    ///   "tipoNotificacion": "info"
    /// }
    /// </code>
    /// <para><b>200 OK</b></para>
    /// <code language="json">
    /// {
    ///   "recipient": "admin@localhost.dev",
    ///   "notificationType": "info",
    ///   "sent": true
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("enviar")]
    [ProducesResponseType<SendNotificationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SendNotificationResponse>> SendNotificationAsync(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var command = request.ToCommand();

        Result result = await Sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: () =>
            {
                var response = request.ToResponse();
                return Ok(response);
            },
            onFailure: _ => ProblemFromResult<SendNotificationResponse>(result));
    }
}
