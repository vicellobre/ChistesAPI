using Bogus;
using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Jokes.ValueObjects;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Topics.Entities;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Repositories;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enjoy.Persistence.Seeding;

public static class DevelopmentDataSeeder
{
    private const string DemoAuthorId = "seed_dev";

    public static async Task SeedAsync(
        IServiceProvider services,
        ILogger logger,
        bool seedIdentityRoles,
        bool seedDevelopmentUsers,
        IReadOnlyList<DevelopmentUserSeedEntry>? developmentUsers,
        bool seedDemoData,
        CancellationToken cancellationToken = default)
    {
        if (seedIdentityRoles || seedDevelopmentUsers)
            await SeedIdentityRolesAsync(services, logger);
        else
            logger.LogInformation("Identity role seed skipped (Database:SeedIdentityRoles = false and Database:SeedDevelopmentUsers = false).");

        if (seedDevelopmentUsers)
            await SeedDevelopmentUsersAsync(services, logger, developmentUsers, cancellationToken);
        else
            logger.LogInformation("Development user seed skipped (Database:SeedDevelopmentUsers = false).");

        if (seedDemoData)
            await SeedDemoTopicsAndJokesIfEmptyAsync(services, logger, cancellationToken);
        else
            logger.LogInformation("Demo seed (topics/jokes) skipped (Database:SeedDemoData = false).");
    }

    private static async Task SeedDevelopmentUsersAsync(
        IServiceProvider services,
        ILogger logger,
        IReadOnlyList<DevelopmentUserSeedEntry>? developmentUsers,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<DevelopmentUserSeedEntry> entries = developmentUsers ?? [];

        if (entries.Count == 0)
        {
            logger.LogInformation(
                "User seed: no entries in Database:DevelopmentUsers (empty or missing list).");
            return;
        }

        IUserRegistrationService registration = services.GetRequiredService<IUserRegistrationService>();
        IUserRepository userRepository = services.GetRequiredService<IUserRepository>();
        UserManager<IdentityUser> userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        try
        {
            foreach (DevelopmentUserSeedEntry entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Email) || string.IsNullOrWhiteSpace(entry.Password))
                {
                    logger.LogWarning("Seed user entry ignored: Email or Password is empty.");
                    continue;
                }

                string email = entry.Email.Trim();
                string roleName = string.IsNullOrWhiteSpace(entry.Role)
                    ? Role.User
                    : entry.Role.Trim();

                IdentityUser? identityExisting = await userManager.FindByEmailAsync(email);
                if (identityExisting is not null)
                {
                    logger.LogInformation("Seed user skipped: {Email} already exists in Identity.", email);
                    continue;
                }

                User? existingDomain = await userRepository.GetByEmailAsync(email);
                if (existingDomain is not null)
                {
                    logger.LogInformation("Seed user skipped: {Email} already exists in domain.", email);
                    continue;
                }

                Result<AuthResult> result = await registration.RegisterAsync(
                    string.IsNullOrWhiteSpace(entry.Name) ? email : entry.Name.Trim(),
                    email,
                    entry.Password,
                    roleName,
                    cancellationToken);

                if (result.IsFailure)
                {
                    string message = string.Join("; ", result.Errors.Select(e => e.Message));
                    throw new InvalidOperationException($"Could not create seed user '{email}': {message}");
                }

                logger.LogInformation(
                    "Development user created: {Email} (role {Role}).",
                    email,
                    roleName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding development users.");
            throw;
        }
    }

    private static async Task SeedIdentityRolesAsync(IServiceProvider services, ILogger logger)
    {
        RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            foreach (string roleName in Role.All)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;

                IdentityResult created = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!created.Succeeded)
                {
                    string message = string.Join("; ", created.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Could not create role '{roleName}': {message}");
                }
            }

            logger.LogInformation("Identity roles verified or created: {Roles}.", string.Join(", ", Role.All));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding Identity roles.");
            throw;
        }
    }

    private static async Task SeedDemoTopicsAndJokesIfEmptyAsync(
        IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        ApplicationDbContext db = services.GetRequiredService<ApplicationDbContext>();

        if (await db.Topics.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Demo seed skipped: topics already exist in the database.");
            return;
        }

        try
        {
            var faker = new Faker();
            var random = new Random();

            var topics = new List<Topic>();
            for (var i = 0; i < 8; i++)
            {
                string name = $"{faker.Commerce.Department()} {faker.Random.AlphaNumeric(6)}";
                Result<Topic> topicResult = Topic.Create(name);
                if (topicResult.IsSuccess)
                    topics.Add(topicResult.Value);
            }

            await db.Topics.AddRangeAsync(topics, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            if (topics.Count == 0)
            {
                logger.LogWarning("Demo seed: no valid topics were created; skipping jokes.");
                return;
            }

            var jokes = new List<Joke>();
            for (var i = 0; i < 25; i++)
            {
                string text = faker.Lorem.Sentence(faker.Random.Int(8, 24));
                string originValue = Origin.All.ElementAt(random.Next(Origin.All.Count));
                Result<Joke> jokeResult = Joke.Create(text, DemoAuthorId, originValue);
                if (!jokeResult.IsSuccess)
                    continue;

                Joke joke = jokeResult.Value;
                int maxTopics = Math.Min(3, topics.Count);
                int pickCount = faker.Random.Int(1, maxTopics);
                foreach (Topic t in topics.OrderBy(_ => random.Next()).Take(pickCount))
                {
                    joke.AddTopic(t);
                }

                jokes.Add(joke);
            }

            await db.Jokes.AddRangeAsync(jokes, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Demo seed: inserted {TopicCount} topics and {JokeCount} jokes.",
                topics.Count,
                jokes.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding demo data (topics/jokes).");
            throw;
        }
    }
}
