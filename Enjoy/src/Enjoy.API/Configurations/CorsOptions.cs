namespace Enjoy.API.Configurations;

public sealed class CorsOptions
{
    public const string PolicyName = "CorsPolicy";
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; init; } = [];

    public bool AllowAnyOriginWhenEmpty { get; init; }
}
