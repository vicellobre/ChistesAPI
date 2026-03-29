using Enjoy.Domain.Shared.Messaging;

namespace Enjoy.Domain.Jokes.Events;

public sealed record JokeTextUpdatedDomainEvent(string Id, string JokeId) : DomainEvent(Id);
