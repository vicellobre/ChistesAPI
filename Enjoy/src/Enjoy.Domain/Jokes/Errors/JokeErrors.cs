namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public static class Joke
    {
        public static readonly Error AuthorRequired = Validation("Joke.AuthorRequired", "The joke must have an author (AuthorId).");

        public static readonly Func<string, Error> NotFound = id => Error.NotFound("Joke.NotFound", $"The joke with id {id} was not found.");

        public static readonly Error TopicIdRequired = Validation("Joke.TopicIdRequired", "The topic id cannot be empty when associating.");
    }

    public static class JokeText
    {
        public static readonly Error IsNullOrEmpty = Validation("JokeText.IsNullOrEmpty", "The joke text cannot be empty.");

        public static readonly Func<int, Error> TooLong = length => Validation("JokeText.TooLong", $"The joke text cannot exceed {length} characters.");
    }

    public static class Origin
    {
        public static readonly Error IsNullOrEmpty = Validation("Origin.IsNullOrEmpty", "The joke origin cannot be empty.");

        public static readonly Error Invalid = Validation("Origin.Invalid", "The specified origin is not valid.");
    }
}
