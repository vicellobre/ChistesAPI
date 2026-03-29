using Enjoy.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace Enjoy.Application.Services;

internal sealed class SmsNotifier(ILogger<SmsNotifier> logger) : INotifier
{
    public NotificationChannel Channel => NotificationChannel.Sms;

    public Task NotifyAsync(string recipient, string message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[SMS] To: {Recipient} — {Message}", recipient, message);
        return Task.CompletedTask;
    }
}
