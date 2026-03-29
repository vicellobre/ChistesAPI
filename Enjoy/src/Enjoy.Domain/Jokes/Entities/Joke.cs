using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;
using Enjoy.Domain.Shared.Messaging;
using Enjoy.Domain.Shared.Primitives;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Jokes.Events;
using Enjoy.Domain.Jokes.ValueObjects;
using Enjoy.Domain.Topics.Entities;

namespace Enjoy.Domain.Jokes.Entities;

public sealed class Joke : AggregateRoot, IAuditableEntity
{
    public static string NewId() => $"joke_{Guid.CreateVersion7()}";

    public JokeText Text { get; private set; }
    public string AuthorId { get; private set; }
    public Origin Origin { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    private readonly List<Topic> _topics = [];
    public IReadOnlyCollection<Topic> Topics => _topics.AsReadOnly();

    public IReadOnlyCollection<string> TopicIds => _topics.Select(t => t.Id).ToList();

    private Joke() : base()
    {
        AuthorId = null!;
        Text = default;
        Origin = default;
    }

    private Joke(string id, JokeText text, string authorId, Origin origin) : base(id)
    {
        Text = text;
        AuthorId = authorId;
        Origin = origin;
    }

    public static Result<Joke> Create(string text, string authorId, string origin)
    {
        List<Error> errors = [];

        if (string.IsNullOrWhiteSpace(authorId))
            errors.Add(Error.Joke.AuthorRequired);

        var textResult = JokeText.Create(text);
        if (textResult.IsFailure)
            errors.AddRange(textResult.Errors);

        var originResult = Origin.Create(origin);
        if (originResult.IsFailure)
            errors.AddRange(originResult.Errors);

        if (!errors.IsEmpty())
            return Result<Joke>.Failure(errors);

        Joke joke = new(
            NewId(),
            textResult.Value,
            authorId.Trim(),
            originResult.Value);
        joke.RaiseDomainEvent(new JokeCreatedDomainEvent(DomainEvent.NewId(), joke.Id, joke.AuthorId));
        return Result<Joke>.Success(joke);
    }

    public Result UpdateText(string text)
    {
        var textResult = JokeText.Create(text);
        if (textResult.IsFailure)
            return Result.Failure(textResult.Errors);
        if (Text.Value == textResult.Value.Value)
            return Result.Success();

        Text = textResult.Value;
        RaiseDomainEvent(new JokeTextUpdatedDomainEvent(DomainEvent.NewId(), Id));
        return Result.Success();
    }

    public Result AddTopic(Topic topic)
    {
        if (topic is null)
            return Result.Failure(Error.Joke.TopicIdRequired);
        if (_topics.Any(t => t.Id == topic.Id))
            return Result.Success();

        _topics.Add(topic);
        return Result.Success();
    }

    public Result RemoveTopic(string topicId)
    {
        if (string.IsNullOrWhiteSpace(topicId))
            return Result.Success();

        _topics.RemoveAll(t => t.Id == topicId);
        return Result.Success();
    }
}
