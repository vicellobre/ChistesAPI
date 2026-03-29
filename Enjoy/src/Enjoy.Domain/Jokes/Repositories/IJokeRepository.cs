using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Shared.Abstractions;

namespace Enjoy.Domain.Jokes.Repositories;

public interface IJokeRepository : IRepository<Joke>
{
    Task AddAsync(Joke joke);
    Task<Joke?> GetByIdAsync(string id);
    Task UpdateAsync(Joke joke);
    Task RemoveAsync(string id);
    Task<IReadOnlyCollection<Joke>> ListAsync();
}