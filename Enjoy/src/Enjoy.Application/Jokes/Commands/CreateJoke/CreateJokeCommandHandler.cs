using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Jokes.Repositories;
using Enjoy.Domain.Jokes.ValueObjects;
using Enjoy.Domain.Shared.Abstractions;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Jokes.Commands.CreateJoke;

internal sealed class CreateJokeCommandHandler(
    IJokeRepository jokeRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateJokeCommand, CreateJokeCommandResponse>
{
    public async Task<Result<CreateJokeCommandResponse>> Handle(
        CreateJokeCommand request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Failure<CreateJokeCommandResponse>(
                Error.Unauthorized("Auth.Unauthenticated", "User is not authenticated."));

        var jokeResult = Joke.Create(request.Text, userId, Origin.Local);

        if (jokeResult.IsFailure)
            return Result.Failure<CreateJokeCommandResponse>(jokeResult.Errors);

        await jokeRepository.AddAsync(jokeResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CreateJokeCommandResponse(jokeResult.Value.Id));
    }
}
