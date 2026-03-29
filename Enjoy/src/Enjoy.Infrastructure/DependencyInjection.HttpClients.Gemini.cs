using Enjoy.Application.Abstractions.Jokes;
using Enjoy.Infrastructure.Jokes;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructureGeminiHttpClient(this IServiceCollection services)
    {
        services.AddScoped<IJokeFusionService, GeminiJokeFusionService>();
        return services;
    }
}
