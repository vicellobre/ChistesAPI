namespace Enjoy.Application.Abstractions.Jokes;

public interface IChuckNorrisJokeService
{
    Task<string?> GetRandomJokeAsync(CancellationToken cancellationToken = default);
}
