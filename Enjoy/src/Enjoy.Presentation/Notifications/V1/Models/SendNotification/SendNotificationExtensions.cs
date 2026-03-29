using Enjoy.Application.Notifications.Commands.SendNotification;

namespace Enjoy.Presentation.Notifications.V1.Models.SendNotification;

public static class SendNotificationExtensions
{
    public static SendNotificationCommand ToCommand(this SendNotificationRequest request) =>
        new(request.Recipient, request.Message, request.NotificationType);

    public static SendNotificationResponse ToResponse(this SendNotificationRequest request) =>
        new(request.Recipient, request.NotificationType);
}
