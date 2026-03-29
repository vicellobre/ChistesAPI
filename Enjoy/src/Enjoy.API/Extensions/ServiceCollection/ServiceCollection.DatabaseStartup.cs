using Enjoy.API.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.API.Extensions.ServiceCollection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseStartupOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseStartupOptions>(configuration.GetSection(DatabaseStartupOptions.SectionName));
        return services;
    }
}
