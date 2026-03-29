using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enjoy.Application.Abstractions.Jokes;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Infrastructure.Jokes.Options;
using Google.GenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enjoy.Infrastructure.Jokes;

public sealed class GeminiJokeFusionService : IJokeFusionService
{
    private static readonly JsonSerializerOptions BatchFusionJsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IOptionsMonitor<GeminiOptions> _options;
    private readonly ILogger<GeminiJokeFusionService> _logger;

    public GeminiJokeFusionService(
        IOptionsMonitor<GeminiOptions> options,
        ILogger<GeminiJokeFusionService> logger)
    {
        _options = options;
        _logger = logger;
    }

    public Task<Result<string>> FuseTwoJokesAsync(
        string firstJoke,
        string secondJoke,
        CancellationToken cancellationToken = default)
    {
        string prompt =
            "Write ONE new joke that blends ideas from both (your own punchline, wordplay, or a bridge between the two). " +
            "Do not concatenate the two texts with a period or place them back-to-back unchanged. " +
            $"Joke 1: '{firstJoke}' Joke 2: '{secondJoke}'. " +
            "Reply with only that joke, no explanations.";

        return GenerateAsync(prompt, cancellationToken);
    }

    public Task<Result<string>> FuseFragmentsAsync(
        IReadOnlyList<string> fragments,
        CancellationToken cancellationToken = default)
    {
        if (fragments.Count == 0)
        {
            return Task.FromResult(Result<string>.Failure(
                Error.Failure("JokeFusion.NoFragments", "There are no fragments to combine.")));
        }

        var lines = string.Join(
            " ",
            fragments.Select((f, i) => $"Fragment {i + 1}: \"{f.Trim()}\""));

        string prompt =
            "Creatively combine the following fragments into one short, funny joke—blend ideas; do not simply concatenate the fragments. " +
            lines +
            " Reply with only the final joke, no explanations or titles.";

        return GenerateAsync(prompt, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<string>>> FusePairedJokesBatchAsync(
        IReadOnlyList<(string Chuck, string Dad)> pairs,
        CancellationToken cancellationToken = default)
    {
        if (pairs.Count == 0)
        {
            return Result<IReadOnlyList<string>>.Failure(
                Error.Failure("JokeFusion.NoPairs", "There are no joke pairs to fuse."));
        }

        string prompt = BuildBatchFusionPrompt(pairs);
        Result<string> raw = await GenerateAsync(prompt, cancellationToken).ConfigureAwait(false);
        if (raw.IsFailure)
        {
            return Result<IReadOnlyList<string>>.Failure(raw.FirstError);
        }

        return ParseBatchFusionJson(raw.Value, pairs.Count);
    }

    private static string BuildBatchFusionPrompt(IReadOnlyList<(string Chuck, string Dad)> pairs)
    {
        int n = pairs.Count;
        var sb = new StringBuilder(capacity: 512 + n * 128);

        sb.AppendLine("Reply with JSON only (no markdown, no extra text). Exact shape:");
        sb.AppendLine("{\"combinations\":[\"...\",\"...\",...]}");
        sb.AppendLine(
            CultureInfo.InvariantCulture,
            $"The array must contain exactly {n} strings, in the same order as the pairs below.");
        sb.AppendLine("Each string must be ONE new joke that blends both ideas (absurd tone, a single punchline, wordplay, or a bridge between the two).");
        sb.AppendLine("Forbidden: pasting the Chuck text then the Dad text with a period or space; forbidden to return both jokes back-to-back without original wording.");
        sb.AppendLine("Keep each fusion at most one or two sentences; do not simply concatenate the two source jokes.");
        sb.AppendLine();

        for (int i = 0; i < n; i++)
        {
            (string chuck, string dad) = pairs[i];
            sb.Append(i + 1).Append(") Chuck: ").AppendLine(FormatJokeForPrompt(chuck));
            sb.Append("   Dad: ").AppendLine(FormatJokeForPrompt(dad));
        }

        return sb.ToString();
    }

    private static string FormatJokeForPrompt(string joke)
    {
        if (string.IsNullOrEmpty(joke))
        {
            return "(empty)";
        }

        return joke.ReplaceLineEndings(" ").Trim();
    }

    private Result<IReadOnlyList<string>> ParseBatchFusionJson(string text, int expectedCount)
    {
        string payload = ExtractJsonObject(text);

        try
        {
            BatchFusionEnvelopeDto? dto = JsonSerializer.Deserialize<BatchFusionEnvelopeDto>(
                payload,
                BatchFusionJsonSerializerOptions);

            if (dto?.Combinations is null || dto.Combinations.Count != expectedCount)
            {
                _logger.LogWarning(
                    "Gemini batch fusion: expected {Expected} combinations; got {Actual}.",
                    expectedCount,
                    dto?.Combinations?.Count ?? -1);

                return Result<IReadOnlyList<string>>.Failure(
                    Error.Failure(
                        "JokeFusion.BatchInvalidJson",
                        "Gemini's response does not contain the expected number of combinations."));
            }

            var list = new List<string>(expectedCount);

            for (int i = 0; i < expectedCount; i++)
            {
                string? line = dto.Combinations[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    return Result<IReadOnlyList<string>>.Failure(
                        Error.Failure(
                            "JokeFusion.BatchInvalidJson",
                            $"The combination at position {i + 1} is empty."));
                }

                list.Add(line.Trim());
            }

            return Result<IReadOnlyList<string>>.Success(list);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(
                ex,
                "Gemini batch fusion: invalid JSON. Snippet: {Snippet}",
                TruncateForProblemDetail(payload, 400));

            return Result<IReadOnlyList<string>>.Failure(
                Error.Failure(
                    "JokeFusion.BatchInvalidJson",
                    "Could not parse the JSON returned by Gemini."));
        }
    }

    private static string ExtractJsonObject(string text)
    {
        text = text.Trim();

        if (text.StartsWith("```", StringComparison.Ordinal))
        {
            int firstNewline = text.IndexOf('\n');
            int lastFence = text.LastIndexOf("```", StringComparison.Ordinal);
            if (firstNewline >= 0 && lastFence > firstNewline)
            {
                text = text.Substring(firstNewline + 1, lastFence - firstNewline - 1).Trim();
            }
        }

        int firstBrace = text.IndexOf('{');
        int lastBrace = text.LastIndexOf('}');
        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            return text.Substring(firstBrace, lastBrace - firstBrace + 1);
        }

        return text;
    }

    private async Task<Result<string>> GenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        GeminiOptions opt = _options.CurrentValue;
        
        string? apiKey = !string.IsNullOrWhiteSpace(opt.ApiKey)
            ? opt.ApiKey.Trim()
            : Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Result<string>.Failure(
                Error.Failure(
                    "JokeFusion.ApiKeyMissing",
                    "Set Gemini:ApiKey (appsettings, user secrets) or the GEMINI_API_KEY environment variable."));
        }

        string model = string.IsNullOrWhiteSpace(opt.ModelId) ? "gemma-3-12b-it" : opt.ModelId.Trim();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var client = new Client(apiKey: apiKey);
            var response = await client.Models.GenerateContentAsync(
                model: model,
                contents: prompt,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            string? text = response.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
            text = text?.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                return Result<string>.Failure(
                    Error.Failure(
                        "JokeFusion.EmptyModelResponse",
                        "Gemini returned no usable text. Check the model or blocked content."));
            }

            return Result<string>.Success(text);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ClientError ex)
        {
            _logger.LogWarning(ex, "Gemini ClientError (SDK Google.GenAI)");
            return Result<string>.Failure(MapGeminiClientError(ex));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini generateContent (Google.GenAI SDK)");
            return Result<string>.Failure(
                Error.Failure(
                    "JokeFusion.GeminiHttpError",
                    "Error calling Gemini. Check model, quota, and API key."));
        }
    }

    private static Error MapGeminiClientError(ClientError ex)
    {
        string detail = TruncateForProblemDetail(ex.Message);

        if (IsGeminiQuotaOrRateLimit(ex.Message))
        {
            return Error.TooManyRequests(
                "JokeFusion.GeminiQuotaExceeded",
                detail);
        }

        return Error.Failure("JokeFusion.GeminiHttpError", detail);
    }

    private static bool IsGeminiQuotaOrRateLimit(string message)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        if (message.Contains("RESOURCE_EXHAUSTED", StringComparison.OrdinalIgnoreCase))
            return true;

        if (message.Contains("exceeded your current quota", StringComparison.OrdinalIgnoreCase))
            return true;

        if (message.Contains("Quota exceeded", StringComparison.OrdinalIgnoreCase))
            return true;

        if (message.Contains("quota", StringComparison.OrdinalIgnoreCase)
            && message.Contains("exceeded", StringComparison.OrdinalIgnoreCase))
            return true;

        return message.Contains("generate_content_free_tier", StringComparison.OrdinalIgnoreCase)
            && message.Contains("limit: 0", StringComparison.OrdinalIgnoreCase);
    }

    private static string TruncateForProblemDetail(string message, int maxLength = 2500)
    {
        if (string.IsNullOrEmpty(message))
            return "No detail from provider.";

        return message.Length <= maxLength
            ? message
            : string.Concat(message.AsSpan(0, maxLength), "…");
    }

    private sealed class BatchFusionEnvelopeDto
    {
        [JsonPropertyName("combinations")]
        public List<string>? Combinations { get; set; }
    }
}
