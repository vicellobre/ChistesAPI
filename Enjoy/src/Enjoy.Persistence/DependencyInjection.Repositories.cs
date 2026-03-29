using Enjoy.Domain.Jokes.Repositories;
using Enjoy.Domain.Shared.Abstractions;
using Enjoy.Domain.Topics.Repositories;
using Enjoy.Domain.Users.Repositories;
using Enjoy.Persistence.Jokes;
using Enjoy.Persistence.Topics;
using Enjoy.Persistence.UnitOfWorks;
using Enjoy.Persistence.Users;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Enjoy.Persistence;

public static partial class DependencyInjection
{
    public static IServiceCollection AddPersistenceRepositories(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.Decorate<IUserRepository, CachedUserRepository>();
        services.AddScoped<IJokeRepository, JokeRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();

        return services;
    }
}
