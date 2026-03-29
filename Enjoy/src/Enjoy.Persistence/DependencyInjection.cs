using Enjoy.Domain.Shared.Exceptions;
using Enjoy.Persistence.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Persistence;

public static partial class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConfigurationKeys.ConnectionEnjoy)
            ?? throw new MissingConnectionStringException(ConfigurationKeys.ConnectionEnjoy);

        services.AddPersistenceInterceptors();
        services.AddPersistenceRepositories();
        services.AddPersistenceDbContext(connectionString);

        return services;
    }
}
