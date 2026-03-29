namespace Enjoy.Infrastructure.HttpClients.Options;

public sealed class IntegrationHttpClientsOptions
{
    public const string SectionName = "IntegrationHttpClients";

    public ExternalAuthHttpClientOptions ExternalAuth { get; set; } = new();

    public ChuckNorrisHttpClientOptions ChuckNorris { get; set; } = new();

    public DadJokeHttpClientOptions DadJoke { get; set; } = new();
}

public sealed class ExternalAuthHttpClientOptions
{
    public string AcceptHeaderName { get; set; } = "Accept";

    public string AcceptHeaderValue { get; set; } = "application/json";

    public int RequestTimeoutSeconds { get; set; } = 100;

    public int RequestTimeoutMinSeconds { get; set; } = 5;

    public int RequestTimeoutMaxSeconds { get; set; } = 300;
}

public sealed class ChuckNorrisHttpClientOptions
{
    public const string DefaultBaseUrl = "https://api.chucknorris.io/";

    public string BaseUrl { get; set; } = DefaultBaseUrl;

    public int RequestTimeoutSeconds { get; set; } = 30;

    public int RequestTimeoutMinSeconds { get; set; } = 5;

    public int RequestTimeoutMaxSeconds { get; set; } = 300;
}

public sealed class DadJokeHttpClientOptions
{
    public const string DefaultBaseUrl = "https://icanhazdadjoke.com/";

    public string UserAgent { get; set; } =
        "EnjoyApp/1.0 (+https://localhost; replace with project URL or email)";

    public string BaseUrl { get; set; } = DefaultBaseUrl;

    public string AcceptMediaType { get; set; } = "application/json";

    public int RequestTimeoutSeconds { get; set; } = 30;

    public int RequestTimeoutMinSeconds { get; set; } = 5;

    public int RequestTimeoutMaxSeconds { get; set; } = 300;
}
