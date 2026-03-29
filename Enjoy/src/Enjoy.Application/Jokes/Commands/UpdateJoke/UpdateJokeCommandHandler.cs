using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Jokes.Repositories;
using Enjoy.Domain.Shared.Abstractions;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Application.Jokes.Commands.UpdateJoke;

internal sealed class UpdateJokeCommandHandler(
    IJokeRepository jokeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateJokeCommand>
{
    public async Task<Result> Handle(
        UpdateJokeCommand request,
        CancellationToken cancellationToken)
    {
        var joke = await jokeRepository.GetByIdAsync(request.Id);
        if (joke is null)
            return Result.Failure(Error.Joke.NotFound(request.Id));

        if (joke.AuthorId != request.RequesterId && request.RequesterRole != Role.Admin)
            return Result.Failure(Error.Forbidden("Joke.UpdateForbidden", "Only the author or an admin can update this joke."));

        var updateResult = joke.UpdateText(request.NewText);
        if (updateResult.IsFailure)
            return updateResult;

        await jokeRepository.UpdateAsync(joke);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
