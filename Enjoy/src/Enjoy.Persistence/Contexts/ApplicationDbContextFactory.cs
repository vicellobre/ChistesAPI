using Enjoy.Domain.Shared.Exceptions;
using Enjoy.Persistence.Constants;
using Enjoy.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Enjoy.Persistence.Contexts;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private readonly IConfiguration? _configuration;

    public ApplicationDbContextFactory()
    {
        _configuration = BuildConfiguration();
    }

    public ApplicationDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = _configuration ?? BuildConfiguration();
        var connectionString = configuration.GetConnectionString(ConfigurationKeys.ConnectionEnjoy)
            ?? throw new MissingConnectionStringException(ConfigurationKeys.ConnectionEnjoy);

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder
            .UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application))
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(
                new ConvertDomainEventsToOutboxMessagesInterceptor(),
                new UpdateAuditableEntitiesInterceptor());

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static IConfiguration BuildConfiguration()
    {
        var basePath = Directory.GetCurrentDirectory();

        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile(ConfigurationKeys.AppSettingsFileName, optional: true)
            .AddJsonFile(ConfigurationKeys.AppSettingsDevelopmentFileName, optional: true)
            .Build();
    }
}
