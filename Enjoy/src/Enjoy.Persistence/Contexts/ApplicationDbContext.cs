using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Topics.Entities;
using Enjoy.Domain.Users.Entities;
using Enjoy.Persistence.Constants;
using Enjoy.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Enjoy.Persistence.Contexts;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Joke> Jokes => Set<Joke>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers => Set<OutboxMessageConsumer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Application);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
