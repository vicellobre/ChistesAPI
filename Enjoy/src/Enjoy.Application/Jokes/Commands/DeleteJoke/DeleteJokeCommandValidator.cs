using FluentValidation;

namespace Enjoy.Application.Jokes.Commands.DeleteJoke;

internal sealed class DeleteJokeCommandValidator : AbstractValidator<DeleteJokeCommand>
{
    public DeleteJokeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Joke ID is required.");
    }
}
