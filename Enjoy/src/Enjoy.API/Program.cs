using Enjoy.API.Configurations;
using Enjoy.API.Extensions.Database;
using Enjoy.API.Extensions.HostBuilder;
using Enjoy.API.Extensions.OpenTelemetry;
using Enjoy.API.Extensions.Pipeline;
using Enjoy.API.Extensions.ServiceCollection;
using Enjoy.Application;
using Enjoy.Infrastructure;
using Enjoy.Persistence;
using Enjoy.Presentation;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureSerilogFromConfiguration();
builder.AddOpenTelemetryOtlp();

builder.Services
    .AddDatabaseStartupOptions(builder.Configuration)
    .AddPresentationControllers()
    .AddApiVersioningForOpenApi()
    .AddSwaggerDocumentation()
    .AddProblemDetailsConfiguration()
    .AddCorsFromConfiguration(builder.Configuration)
    .AddRateLimiting(builder.Configuration)
    .AddGlobalExceptionHandler()
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();


if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    DatabaseStartupOptions databaseStartup =
        app.Services.GetRequiredService<IOptions<DatabaseStartupOptions>>().Value;

    if (databaseStartup.ApplyMigrationsOnStartup)
        await app.ApplyDatabaseMigrationsAsync();
    else
        app.Logger.LogInformation("Startup migrations skipped (Database:ApplyMigrationsOnStartup = false).");

    if (databaseStartup.SeedIdentityRoles
        || databaseStartup.SeedDevelopmentUsers
        || databaseStartup.SeedDemoData)
        await app.SeedDevelopmentDatabaseAsync(databaseStartup);
}

app.UseHttpPipeline(app.Environment);

await app.RunAsync();
