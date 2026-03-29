using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Repositories;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Persistence.Abstractions;
using Enjoy.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Enjoy.Persistence.Users;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public Task AddAsync(User user)
    {
        Add(user);
        return Task.CompletedTask;
    }

    public async Task<User?> GetByIdAsync(string id) =>
        await base.GetByIdAsync(id, CancellationToken.None);

    public async Task<User?> GetByIdentityIdAsync(Guid identityId) =>
        await Set.FirstOrDefaultAsync(u => u.IdentityId == identityId);

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        Result<Email> created = Email.Create(normalized);
        if (!created.IsSuccess)
            return null;

        Email emailVo = created.Value;
        return await Set.FirstOrDefaultAsync(u => u.Email == emailVo);
    }

    public Task UpdateAsync(User user)
    {
        Update(user);
        return Task.CompletedTask;
    }
}
