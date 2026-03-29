using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Abstractions.Notifications;

public interface IAlertService
{
    Task<Result> SendAlertAsync(
        string recipient,
        string message,
        NotificationChannel channel,
        CancellationToken cancellationToken = default);
}
