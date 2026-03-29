using FluentValidation;

namespace Enjoy.Application.Auth.Commands.ExternalLoginCallback;

internal sealed class ExternalLoginCallbackCommandValidator
    : AbstractValidator<ExternalLoginCallbackCommand>
{
    public ExternalLoginCallbackCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Authorization code is required.");

        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.");
    }
}
