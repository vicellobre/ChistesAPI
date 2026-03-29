using Enjoy.Domain.Topics.Entities;
using Enjoy.Domain.Topics.Repositories;
using Enjoy.Persistence.Abstractions;
using Enjoy.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Enjoy.Persistence.Topics;

public sealed class TopicRepository : Repository<Topic>, ITopicRepository
{
    public TopicRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public Task AddAsync(Topic topic)
    {
        Add(topic);
        return Task.CompletedTask;
    }

    public async Task<Topic?> GetByIdAsync(string id) =>
        await base.GetByIdAsync(id, CancellationToken.None);

    public async Task<Topic?> GetByNameAsync(string name) =>
        await Set.FirstOrDefaultAsync(t => t.Name.Value == name);

    public Task UpdateAsync(Topic topic)
    {
        Update(topic);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(string id)
    {
        var topic = await GetByIdAsync(id);
        if (topic is not null)
            Set.Remove(topic);
    }

    public async Task<IReadOnlyCollection<Topic>> ListAsync() =>
        await Set.ToListAsync();
}
