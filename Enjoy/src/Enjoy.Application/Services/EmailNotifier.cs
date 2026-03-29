using Enjoy.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace Enjoy.Application.Services;

internal sealed class EmailNotifier(ILogger<EmailNotifier> logger) : INotifier
{
    public NotificationChannel Channel => NotificationChannel.Email;

    public Task NotifyAsync(string recipient, string message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Email] To: {Recipient} — {Message}", recipient, message);
        return Task.CompletedTask;
    }
}
