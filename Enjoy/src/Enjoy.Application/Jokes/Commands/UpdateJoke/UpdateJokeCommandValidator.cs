using FluentValidation;

namespace Enjoy.Application.Jokes.Commands.UpdateJoke;

internal sealed class UpdateJokeCommandValidator : AbstractValidator<UpdateJokeCommand>
{
    public UpdateJokeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Joke ID is required.");

        RuleFor(x => x.NewText)
            .NotEmpty()
            .WithMessage("New joke text is required.");
    }
}
