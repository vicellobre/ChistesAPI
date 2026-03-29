namespace Enjoy.Infrastructure.Jokes.Options;

public sealed class GeminiOptions
{
    public const string SectionName = "Gemini";

    public string ApiKey { get; set; } = string.Empty;

    public string ModelId { get; set; } = "gemma-3-12b-it";

    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com";

    public int RequestTimeoutSeconds { get; set; } = 60;

    public int RequestTimeoutMinSeconds { get; set; } = 5;

    public int RequestTimeoutMaxSeconds { get; set; } = 300;
}
