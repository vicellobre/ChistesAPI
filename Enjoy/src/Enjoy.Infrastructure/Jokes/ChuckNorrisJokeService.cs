using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Enjoy.Application.Abstractions.Jokes;
using Microsoft.Extensions.Logging;

namespace Enjoy.Infrastructure.Jokes;

internal sealed class ChuckNorrisJokeService(
    HttpClient httpClient,
    ILogger<ChuckNorrisJokeService> logger) : IChuckNorrisJokeService
{
    public async Task<string?> GetRandomJokeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync("jokes/random", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Chuck Norris API returned {StatusCode}",
                    (int)response.StatusCode);
                return null;
            }

            ChuckNorrisRandomJokeDto? dto = await response.Content.ReadFromJsonAsync<ChuckNorrisRandomJokeDto>(
                cancellationToken: cancellationToken);
            string? text = dto?.Value?.Trim();
            return string.IsNullOrEmpty(text) ? null : text;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch random Chuck Norris joke");
            return null;
        }
    }

    private sealed class ChuckNorrisRandomJokeDto
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
