using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Messaging;
using Enjoy.Persistence.Contexts;
using Enjoy.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Enjoy.Infrastructure.Idempotence;

public sealed class IdempotentDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    private readonly INotificationHandler<TDomainEvent> _decorated;
    private readonly ApplicationDbContext _dbContext;

    public IdempotentDomainEventHandler(
        INotificationHandler<TDomainEvent> decorated,
        ApplicationDbContext dbContext)
    {
        _decorated = decorated;
        _dbContext = dbContext;
    }

    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        string consumer = _decorated.GetType().Name;

        if (await _dbContext.Set<OutboxMessageConsumer>()
                .AnyAsync(
                    c => c.Id == notification.Id && c.Name == consumer,
                    cancellationToken))
        {
            return;
        }

        await _decorated.Handle(notification, cancellationToken);

        _dbContext.Set<OutboxMessageConsumer>().Add(
            new OutboxMessageConsumer
            {
                Id = notification.Id,
                Name = consumer,
            });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
