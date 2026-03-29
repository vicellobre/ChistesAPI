namespace Enjoy.Application.Abstractions.Jokes;

public interface IDadJokeService
{
    Task<string?> GetRandomJokeAsync(CancellationToken cancellationToken = default);
}
