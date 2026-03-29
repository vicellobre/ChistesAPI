using System.Text.Json.Serialization;

namespace Enjoy.Presentation.Notifications.V1.Models.SendNotification;

/// <summary>JSON body with Spanish property names (see <c>JsonPropertyName</c>).</summary>
/// <param name="Recipient">Recipient email or id.</param>
/// <param name="Message">Notification body.</param>
/// <param name="NotificationType">Logical type (e.g. <c>info</c>).</param>
/// <remarks>
/// <code language="json">
/// {
///   "destinatario": "user@localhost.dev",
///   "mensaje": "Hello",
///   "tipoNotificacion": "info"
/// }
/// </code>
/// </remarks>
public sealed record SendNotificationRequest(
    [property: JsonPropertyName("destinatario")] string Recipient,
    [property: JsonPropertyName("mensaje")] string Message,
    [property: JsonPropertyName("tipoNotificacion")] string NotificationType);
