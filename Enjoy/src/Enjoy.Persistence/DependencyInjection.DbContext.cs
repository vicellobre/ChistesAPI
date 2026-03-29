using Enjoy.Persistence.Constants;
using Enjoy.Persistence.Contexts;
using Enjoy.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Persistence;

public static partial class DependencyInjection
{
    public static IServiceCollection AddPersistenceDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(
            (serviceProvider, optionsBuilder) =>
            {
                optionsBuilder
                    .UseNpgsql(
                        connectionString,
                        npgsql => npgsql.MigrationsHistoryTable(
                            HistoryRepository.DefaultTableName,
                            Schemas.Application))
                    .UseSnakeCaseNamingConvention()
                    .AddInterceptors(
                        serviceProvider.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>(),
                        serviceProvider.GetRequiredService<UpdateAuditableEntitiesInterceptor>());
            });

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options
                .UseNpgsql(
                    connectionString,
                    npgsql => npgsql.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        Schemas.Identity))
                .UseSnakeCaseNamingConvention());

        return services;
    }
}
