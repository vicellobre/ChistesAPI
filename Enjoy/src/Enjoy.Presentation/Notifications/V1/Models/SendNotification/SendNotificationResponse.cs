namespace Enjoy.Presentation.Notifications.V1.Models.SendNotification;

/// <summary>Echo of the simulated send.</summary>
/// <param name="Recipient">Processed recipient.</param>
/// <param name="NotificationType">Type sent.</param>
/// <param name="Sent">Whether the send was considered successful.</param>
public sealed record SendNotificationResponse(
    string Recipient,
    string NotificationType,
    bool Sent = true);
