using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Jokes.Repositories;
using Enjoy.Domain.Shared.Abstractions;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Application.Jokes.Commands.DeleteJoke;

internal sealed class DeleteJokeCommandHandler(
    IJokeRepository jokeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteJokeCommand>
{
    public async Task<Result> Handle(
        DeleteJokeCommand request,
        CancellationToken cancellationToken)
    {
        var joke = await jokeRepository.GetByIdAsync(request.Id);
        if (joke is null)
            return Result.Failure(Error.Joke.NotFound(request.Id));

        if (joke.AuthorId != request.RequesterId && request.RequesterRole != Role.Admin)
            return Result.Failure(Error.Forbidden("Joke.DeleteForbidden", "Only the author or an admin can delete this joke."));

        await jokeRepository.RemoveAsync(joke.Id);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
