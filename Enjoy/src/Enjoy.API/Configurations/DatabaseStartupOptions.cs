using Enjoy.Persistence.Seeding;

namespace Enjoy.API.Configurations;

public sealed class DatabaseStartupOptions
{
    public const string SectionName = "Database";

    public bool ApplyMigrationsOnStartup { get; init; } = true;

    public bool SeedIdentityRoles { get; init; } = true;

    public bool SeedDevelopmentUsers { get; init; } = true;

    public DevelopmentUserSeedEntry[] DevelopmentUsers { get; init; } = [];

    public bool SeedDemoData { get; init; } = true;
}
