using Enjoy.Domain.Shared.Exceptions;
using Enjoy.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Enjoy.Persistence.Contexts;

public sealed class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
{
    private readonly IConfiguration? _configuration;

    public ApplicationIdentityDbContextFactory()
    {
        _configuration = BuildConfiguration();
    }

    public ApplicationIdentityDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ApplicationIdentityDbContext CreateDbContext(string[] args)
    {
        var configuration = _configuration ?? BuildConfiguration();
        var connectionString = configuration.GetConnectionString(ConfigurationKeys.ConnectionEnjoy)
            ?? throw new MissingConnectionStringException(ConfigurationKeys.ConnectionEnjoy);

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
        optionsBuilder
            .UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Identity))
            .UseSnakeCaseNamingConvention();

        return new ApplicationIdentityDbContext(optionsBuilder.Options);
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
