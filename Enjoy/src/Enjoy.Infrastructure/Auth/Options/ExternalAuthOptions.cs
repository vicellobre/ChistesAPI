namespace Enjoy.Infrastructure.Auth.Options;

public sealed class ExternalAuthOptions
{
    public const string SectionName = "ExternalAuth";

    public GoogleProviderOptions Google { get; init; } = new();
    public GitHubProviderOptions GitHub { get; init; } = new();
}

public sealed class GoogleProviderOptions
{
    public const string ProviderName = "Google";

    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string RedirectUri { get; init; } = string.Empty;
}

public sealed class GitHubProviderOptions
{
    public const string ProviderName = "GitHub";

    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string RedirectUri { get; init; } = string.Empty;
}
