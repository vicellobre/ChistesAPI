using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Jokes.Repositories;
using Enjoy.Persistence.Abstractions;
using Enjoy.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Enjoy.Persistence.Jokes;

public sealed class JokeRepository : Repository<Joke>, IJokeRepository
{
    public JokeRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public Task AddAsync(Joke joke)
    {
        Add(joke);
        return Task.CompletedTask;
    }

    public async Task<Joke?> GetByIdAsync(string id) =>
        await Set
            .Include(j => j.Topics)
            .FirstOrDefaultAsync(j => j.Id == id);

    public Task UpdateAsync(Joke joke)
    {
        Update(joke);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(string id)
    {
        var joke = await GetByIdAsync(id);
        if (joke is not null)
            Set.Remove(joke);
    }

    public async Task<IReadOnlyCollection<Joke>> ListAsync() =>
        await Set.Include(j => j.Topics).ToListAsync();
}
