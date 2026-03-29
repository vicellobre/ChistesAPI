using Enjoy.Application.Abstractions.Email;
using Microsoft.Extensions.Logging;

namespace Enjoy.Infrastructure.Email;

internal sealed class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Email (stub): To={To}, Subject={Subject}, BodyLength={Length}",
            to,
            subject,
            body?.Length ?? 0);
        return Task.CompletedTask;
    }
}
