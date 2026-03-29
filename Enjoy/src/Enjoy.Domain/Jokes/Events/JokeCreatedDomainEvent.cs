using Enjoy.Domain.Shared.Messaging;

namespace Enjoy.Domain.Jokes.Events;

public sealed record JokeCreatedDomainEvent(string Id, string JokeId, string AuthorId) : DomainEvent(Id);
