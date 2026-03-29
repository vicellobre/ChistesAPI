namespace Enjoy.Application.Abstractions.Notifications;

public interface INotifier
{
    NotificationChannel Channel { get; }

    Task NotifyAsync(string recipient, string message, CancellationToken cancellationToken = default);
}
