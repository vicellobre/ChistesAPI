using Enjoy.Application.Abstractions.Jokes;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Jokes.Repositories;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Jokes.Queries.GetRandomJoke;

internal sealed class GetRandomJokeQueryHandler(
    IChuckNorrisJokeService chuckNorrisService,
    IDadJokeService dadJokeService,
    IJokeRepository jokeRepository) : IQueryHandler<GetRandomJokeQuery, GetRandomJokeQueryResponse>
{
    private static readonly Random Random = new();

    public async Task<Result<GetRandomJokeQueryResponse>> Handle(
        GetRandomJokeQuery request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Origin))
        {
            return request.Origin.Trim().ToLowerInvariant() switch
            {
                "chuck" => await GetChuckJokeAsync(cancellationToken),
                "dad" => await GetDadJokeAsync(cancellationToken),
                _ => Result.Failure<GetRandomJokeQueryResponse>(Error.Origin.Invalid)
            };
        }

        int source = Random.Next(3);
        return source switch
        {
            0 => await GetChuckJokeAsync(cancellationToken),
            1 => await GetDadJokeAsync(cancellationToken),
            _ => await GetLocalJokeAsync(cancellationToken)
        };
    }

    private async Task<Result<GetRandomJokeQueryResponse>> GetChuckJokeAsync(CancellationToken cancellationToken)
    {
        string? joke = await chuckNorrisService.GetRandomJokeAsync(cancellationToken);
        return joke is null
            ? Result.Failure<GetRandomJokeQueryResponse>(
                Error.Failure("Joke.ExternalServiceUnavailable", "Chuck Norris joke service is unavailable."))
            : Result.Success(new GetRandomJokeQueryResponse(joke, "ChuckNorris"));
    }

    private async Task<Result<GetRandomJokeQueryResponse>> GetDadJokeAsync(CancellationToken cancellationToken)
    {
        string? joke = await dadJokeService.GetRandomJokeAsync(cancellationToken);
        return joke is null
            ? Result.Failure<GetRandomJokeQueryResponse>(
                Error.Failure("Joke.ExternalServiceUnavailable", "Dad Joke service is unavailable."))
            : Result.Success(new GetRandomJokeQueryResponse(joke, "DadJoke"));
    }

    private async Task<Result<GetRandomJokeQueryResponse>> GetLocalJokeAsync(CancellationToken cancellationToken)
    {
        var jokes = await jokeRepository.ListAsync();
        if (jokes.Count == 0)
            return await GetExternalJokeWhenLocalEmptyAsync(cancellationToken);

        var joke = jokes.ElementAt(Random.Next(jokes.Count));
        return Result.Success(new GetRandomJokeQueryResponse(joke.Text.Value, joke.Origin.Value));
    }

    private async Task<Result<GetRandomJokeQueryResponse>> GetExternalJokeWhenLocalEmptyAsync(
        CancellationToken cancellationToken)
    {
        var tryChuckFirst = Random.Next(2) == 0;
        var primary = tryChuckFirst
            ? await GetChuckJokeAsync(cancellationToken)
            : await GetDadJokeAsync(cancellationToken);
        if (primary.IsSuccess)
            return primary;

        return tryChuckFirst
            ? await GetDadJokeAsync(cancellationToken)
            : await GetChuckJokeAsync(cancellationToken);
    }
}
