using Enjoy.Application.Abstractions.Notifications;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Notifications.Commands.SendNotification;

internal sealed class SendNotificationCommandHandler(
    IAlertService alertService) : ICommandHandler<SendNotificationCommand>
{
    public async Task<Result> Handle(
        SendNotificationCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<NotificationChannel>(request.NotificationChannel, ignoreCase: true, out var channel))
            return Result.Failure(Error.Validation(
                "Notification.InvalidChannel",
                $"Invalid notification channel '{request.NotificationChannel}'. Valid values: Email, Sms."));

        return await alertService.SendAlertAsync(
            request.Recipient, request.Message, channel, cancellationToken);
    }
}
