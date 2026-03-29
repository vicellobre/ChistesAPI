using Enjoy.Application.Abstractions.Notifications;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Services;

internal sealed class AlertService(IEnumerable<INotifier> notifiers) : IAlertService
{
    public async Task<Result> SendAlertAsync(
        string recipient,
        string message,
        NotificationChannel channel,
        CancellationToken cancellationToken = default)
    {
        var notifier = notifiers.FirstOrDefault(n => n.Channel == channel);

        if (notifier is null)
            return Result.Failure(Error.Failure(
                "Notification.ChannelNotSupported",
                $"No notifier registered for channel '{channel}'."));

        await notifier.NotifyAsync(recipient, message, cancellationToken);

        return Result.Success();
    }
}
