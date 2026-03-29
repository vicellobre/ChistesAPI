using Enjoy.Application.Abstractions.Jokes;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Jokes.Repositories;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Microsoft.Extensions.Logging;

namespace Enjoy.Application.Jokes.Queries.GetCombinedJoke;

internal sealed class GetCombinedJokeQueryHandler(
    IChuckNorrisJokeService chuckNorrisService,
    IDadJokeService dadJokeService,
    IJokeRepository jokeRepository,
    IJokeFusionService jokeFusionService,
    ILogger<GetCombinedJokeQueryHandler> logger) : IQueryHandler<GetCombinedJokeQuery, GetCombinedJokeQueryResponse>
{
    private static readonly Random Random = new();

    public async Task<Result<GetCombinedJokeQueryResponse>> Handle(
        GetCombinedJokeQuery request,
        CancellationToken cancellationToken)
    {
        var fragments = new List<string>();
        var sourceFragments = new List<CombinedJokeSourceFragment>();

        var chuckTask = SafeGetAsync(() => chuckNorrisService.GetRandomJokeAsync(cancellationToken));
        var dadTask = SafeGetAsync(() => dadJokeService.GetRandomJokeAsync(cancellationToken));

        await Task.WhenAll(chuckTask, dadTask);

        if (chuckTask.Result is not null)
        {
            string fragment = ExtractFragment(chuckTask.Result);
            fragments.Add(fragment);
            sourceFragments.Add(new CombinedJokeSourceFragment("ChuckNorris", fragment));
        }

        if (dadTask.Result is not null)
        {
            string fragment = ExtractFragment(dadTask.Result);
            fragments.Add(fragment);
            sourceFragments.Add(new CombinedJokeSourceFragment("DadJoke", fragment));
        }

        var localJokes = await jokeRepository.ListAsync();
        if (localJokes.Count > 0)
        {
            var localJoke = localJokes.ElementAt(Random.Next(localJokes.Count));
            string fragment = ExtractFragment(localJoke.Text.Value);
            fragments.Add(fragment);
            sourceFragments.Add(new CombinedJokeSourceFragment("Local", fragment));
        }

        if (fragments.Count == 0)
        {
            return Result.Failure<GetCombinedJokeQueryResponse>(
                Error.Failure("Joke.NoCombinationPossible", "Could not retrieve jokes from any source."));
        }

        Result<string> fusion = await jokeFusionService.FuseFragmentsAsync(fragments, cancellationToken);
        if (fusion.IsFailure)
        {
            return Result.Failure<GetCombinedJokeQueryResponse>(fusion.FirstError);
        }

        return Result.Success(new GetCombinedJokeQueryResponse(fusion.Value, sourceFragments));
    }

    private async Task<string?> SafeGetAsync(Func<Task<string?>> factory)
    {
        try
        {
            return await factory();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch joke for combination");
            return null;
        }
    }

    private static string ExtractFragment(string joke)
    {
        string trimmed = joke.TrimEnd('.', '!', '?', ' ');
        if (trimmed.Length > 120)
        {
            trimmed = trimmed[..120];
        }

        return trimmed;
    }
}
