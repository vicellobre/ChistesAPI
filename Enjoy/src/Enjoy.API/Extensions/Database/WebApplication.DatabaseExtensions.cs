using Enjoy.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Enjoy.API.Extensions.Database;

public static class WebApplicationDatabaseExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        await using ApplicationDbContext applicationDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await using ApplicationIdentityDbContext identityDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        try
        {
            await applicationDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Application schema migrations applied successfully.");

            await identityDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Identity schema migrations applied successfully.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "Error applying database migrations.");
            throw;
        }
    }
}
