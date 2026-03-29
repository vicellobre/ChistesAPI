using Enjoy.Domain.Shared.Messaging;

namespace Enjoy.Domain.Topics.Events;

public sealed record TopicCreatedDomainEvent(string Id, string TopicId) : DomainEvent(Id);
