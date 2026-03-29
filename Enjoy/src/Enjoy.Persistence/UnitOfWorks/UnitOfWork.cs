using Enjoy.Domain.Shared.Abstractions;
using Enjoy.Persistence.Contexts;

namespace Enjoy.Persistence.UnitOfWorks;

public sealed class UnitOfWork : IUnitOfWork
{
    private bool _disposed;
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }
}
