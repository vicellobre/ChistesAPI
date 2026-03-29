using Enjoy.API.Configurations;
using Enjoy.Persistence.Seeding;

namespace Enjoy.API.Extensions.Database;

public static class WebApplicationDevelopmentDataExtensions
{
    public static async Task SeedDevelopmentDatabaseAsync(this WebApplication app, DatabaseStartupOptions options)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await DevelopmentDataSeeder.SeedAsync(
            scope.ServiceProvider,
            app.Logger,
            options.SeedIdentityRoles,
            options.SeedDevelopmentUsers,
            options.DevelopmentUsers,
            options.SeedDemoData);
    }
}
