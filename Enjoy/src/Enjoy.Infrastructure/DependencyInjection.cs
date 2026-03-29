using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureOptionsAndResilience(configuration);
        services.AddInfrastructureGeminiHttpClient();
        services.AddInfrastructureAuthenticationAndIdentity();
        services.AddInfrastructureIntegrationHttpClients();
        services.AddInfrastructureDataAndEmailServices();
        services.AddInfrastructureDomainEventPipeline();
        services.AddInfrastructureBackgroundJobs(configuration);

        return services;
    }
}
