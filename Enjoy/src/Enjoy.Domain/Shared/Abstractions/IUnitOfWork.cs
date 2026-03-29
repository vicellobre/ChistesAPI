namespace Enjoy.Domain.Shared.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
