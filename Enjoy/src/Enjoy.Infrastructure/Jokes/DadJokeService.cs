using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Enjoy.Application.Abstractions.Jokes;
using Microsoft.Extensions.Logging;

namespace Enjoy.Infrastructure.Jokes;

internal sealed class DadJokeService(
    HttpClient httpClient,
    ILogger<DadJokeService> logger) : IDadJokeService
{
    public async Task<string?> GetRandomJokeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync("/", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "icanhazdadjoke API returned {StatusCode}",
                    (int)response.StatusCode);
                return null;
            }

            DadJokeRandomDto? dto = await response.Content.ReadFromJsonAsync<DadJokeRandomDto>(
                cancellationToken: cancellationToken);
            string? text = dto?.Joke?.Trim();
            return string.IsNullOrEmpty(text) ? null : text;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch random Dad joke");
            return null;
        }
    }

    private sealed class DadJokeRandomDto
    {
        [JsonPropertyName("joke")]
        public string? Joke { get; set; }
    }
}
