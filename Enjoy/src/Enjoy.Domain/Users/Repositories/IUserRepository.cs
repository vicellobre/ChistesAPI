using Enjoy.Domain.Shared.Abstractions;
using Enjoy.Domain.Users.Entities;

namespace Enjoy.Domain.Users.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByIdentityIdAsync(Guid identityId);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
}
