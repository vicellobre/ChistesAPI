using Enjoy.Domain.Shared.Primitives;
using Enjoy.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Enjoy.Persistence.Abstractions;

public abstract class Repository<T> where T : Entity
{
    protected readonly ApplicationDbContext Context;
    protected DbSet<T> Set => Context.Set<T>();

    protected Repository(ApplicationDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await Set.FindAsync([id], cancellationToken);

    public virtual void Add(T entity) => Set.Add(entity);

    public virtual void Update(T entity) => Set.Update(entity);
}
