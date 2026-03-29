using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Notifications.Commands.SendNotification;

public sealed record SendNotificationCommand(
    string Recipient,
    string Message,
    string NotificationChannel) : ICommand;
