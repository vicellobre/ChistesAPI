using Enjoy.Application.Abstractions.Jokes;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Results;
using Microsoft.Extensions.Logging;

namespace Enjoy.Application.Jokes.Queries.GetPairedJokes;

internal sealed class GetPairedJokesQueryHandler(
    IChuckNorrisJokeService chuckNorrisService,
    IDadJokeService dadJokeService,
    IJokeFusionService jokeFusionService,
    ILogger<GetPairedJokesQueryHandler> logger) : IQueryHandler<GetPairedJokesQuery, GetPairedJokesQueryResponse>
{
    private const int PairCount = 5;

    public async Task<Result<GetPairedJokesQueryResponse>> Handle(
        GetPairedJokesQuery request,
        CancellationToken cancellationToken)
    {
        var chuckTasks = Enumerable.Range(0, PairCount)
            .Select(_ => SafeGetJokeAsync(() => chuckNorrisService.GetRandomJokeAsync(cancellationToken), "ChuckNorris"));

        var dadTasks = Enumerable.Range(0, PairCount)
            .Select(_ => SafeGetJokeAsync(() => dadJokeService.GetRandomJokeAsync(cancellationToken), "DadJoke"));

        string?[] chuckJokes = await Task.WhenAll(chuckTasks);
        string?[] dadJokes = await Task.WhenAll(dadTasks);

        var pairInputs = Enumerable.Range(0, PairCount)
            .Select(i => (
                Chuck: chuckJokes[i] ?? $"[Chuck Norris joke #{i + 1} unavailable]",
                Dad: dadJokes[i] ?? $"[Dad joke #{i + 1} unavailable]"))
            .ToList();

        Result<IReadOnlyList<string>> fusionBatch =
            await jokeFusionService.FusePairedJokesBatchAsync(pairInputs, cancellationToken);

        if (fusionBatch.IsFailure)
        {
            return Result.Failure<GetPairedJokesQueryResponse>(fusionBatch.FirstError);
        }

        IReadOnlyList<string> combinados = fusionBatch.Value;
        var pairs = new List<PairedJokeItem>(PairCount);
        for (int i = 0; i < PairCount; i++)
        {
            (string chuck, string dad) = pairInputs[i];
            pairs.Add(new PairedJokeItem(chuck, dad, combinados[i]));
        }

        return Result.Success(new GetPairedJokesQueryResponse(pairs));
    }

    private async Task<string?> SafeGetJokeAsync(Func<Task<string?>> factory, string source)
    {
        try
        {
            return await factory();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch joke from {Source}", source);
            return null;
        }
    }
}
