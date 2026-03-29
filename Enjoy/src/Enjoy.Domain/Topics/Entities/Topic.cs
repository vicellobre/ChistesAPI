using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Shared.Messaging;
using Enjoy.Domain.Shared.Primitives;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Topics.Events;
using Enjoy.Domain.Topics.ValueObjects;

namespace Enjoy.Domain.Topics.Entities;

public sealed class Topic : AggregateRoot
{
    public static string NewId() => $"topic_{Guid.CreateVersion7()}";

    public TopicName Name { get; private set; }

    private readonly List<Joke> _jokes = [];
    public IReadOnlyCollection<Joke> Jokes => _jokes.AsReadOnly();

    private Topic() : base() { }

    private Topic(string id, TopicName name) : base(id)
    {
        Name = name;
    }

    public static Result<Topic> Create(string name)
    {
        var nameResult = TopicName.Create(name);
        if (nameResult.IsFailure)
            return Result<Topic>.Failure(nameResult.Errors);

        Topic topic = new(NewId(), nameResult.Value);
        topic.RaiseDomainEvent(new TopicCreatedDomainEvent(DomainEvent.NewId(), topic.Id));
        return Result<Topic>.Success(topic);
    }

    public Result ChangeName(string name)
    {
        var nameResult = TopicName.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure(nameResult.Errors);
        if (Name.Value == nameResult.Value.Value)
            return Result.Success();

        Name = nameResult.Value;
        return Result.Success();
    }
}
