using Enjoy.Domain.Shared.Abstractions;
using Enjoy.Domain.Topics.Entities;

namespace Enjoy.Domain.Topics.Repositories;

public interface ITopicRepository : IRepository<Topic>
{
    Task AddAsync(Topic topic);
    Task<Topic?> GetByIdAsync(string id);
    Task<Topic?> GetByNameAsync(string name);
    Task UpdateAsync(Topic topic);
    Task RemoveAsync(string id);
    Task<IReadOnlyCollection<Topic>> ListAsync();
}
