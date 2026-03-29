using FluentValidation;

namespace Enjoy.Application.Notifications.Commands.SendNotification;

internal sealed class SendNotificationCommandValidator
    : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Recipient)
            .NotEmpty()
            .WithMessage("Recipient is required.");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required.");

        RuleFor(x => x.NotificationChannel)
            .NotEmpty()
            .WithMessage("Notification channel is required.");
    }
}
