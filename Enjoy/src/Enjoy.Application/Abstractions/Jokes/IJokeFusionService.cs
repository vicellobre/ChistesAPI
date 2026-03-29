using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Abstractions.Jokes;

public interface IJokeFusionService
{
    Task<Result<string>> FuseTwoJokesAsync(
        string firstJoke,
        string secondJoke,
        CancellationToken cancellationToken = default);

    Task<Result<string>> FuseFragmentsAsync(
        IReadOnlyList<string> fragments,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<string>>> FusePairedJokesBatchAsync(
        IReadOnlyList<(string Chuck, string Dad)> pairs,
        CancellationToken cancellationToken = default);
}
