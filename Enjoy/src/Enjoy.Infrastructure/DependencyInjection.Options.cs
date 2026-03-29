using Enjoy.Infrastructure.Auth.Options;
using Enjoy.Infrastructure.BackgroundJobs.Options;
using Enjoy.Infrastructure.HttpClients.Options;
using Enjoy.Infrastructure.Jokes.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructureOptionsAndResilience(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<ExternalAuthOptions>(configuration.GetSection(ExternalAuthOptions.SectionName));
        services.Configure<GeminiOptions>(configuration.GetSection(GeminiOptions.SectionName));
        services.Configure<IntegrationHttpClientsOptions>(
            configuration.GetSection(IntegrationHttpClientsOptions.SectionName));
        services.Configure<OutboxProcessorOptions>(configuration.GetSection(OutboxProcessorOptions.SectionName));

        services.ConfigureHttpClientDefaults(static http => http.AddStandardResilienceHandler());

        return services;
    }
}
