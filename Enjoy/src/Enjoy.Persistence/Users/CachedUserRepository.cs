using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Enjoy.Persistence.Users;

public sealed class CachedUserRepository : IUserRepository
{
    private readonly IUserRepository _decorated;
    private readonly IMemoryCache _memoryCache;

    public CachedUserRepository(IUserRepository decorated, IMemoryCache memoryCache)
    {
        _decorated = decorated;
        _memoryCache = memoryCache;
    }

    public Task AddAsync(User user) => _decorated.AddAsync(user);

    public Task<User?> GetByIdAsync(string id)
    {
        string key = $"user-{id}";

        return _memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await _decorated.GetByIdAsync(id);
            });
    }

    public Task<User?> GetByIdentityIdAsync(Guid identityId) =>
        _decorated.GetByIdentityIdAsync(identityId);

    public Task<User?> GetByEmailAsync(string email) =>
        _decorated.GetByEmailAsync(email);

    public Task UpdateAsync(User user) => _decorated.UpdateAsync(user);
}
