namespace Enjoy.Domain.Shared.Messaging;

public abstract record DomainEvent(string Id) : IDomainEvent
{
    public static string NewId() => $"evt_{Guid.CreateVersion7()}";
}
