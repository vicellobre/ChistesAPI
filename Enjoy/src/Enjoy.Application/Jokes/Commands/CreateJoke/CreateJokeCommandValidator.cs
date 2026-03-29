using FluentValidation;

namespace Enjoy.Application.Jokes.Commands.CreateJoke;

internal sealed class CreateJokeCommandValidator : AbstractValidator<CreateJokeCommand>
{
    public CreateJokeCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Joke text is required.");
    }
}
