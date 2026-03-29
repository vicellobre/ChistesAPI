namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public static class Topic
    {
        public static readonly Func<string, Error> NotFound =
            id => Error.NotFound("Topic.NotFound", $"The topic with id {id} was not found.");
    }

    public static class TopicName
    {
        public static readonly Error IsNullOrEmpty =
            Validation("TopicName.IsNullOrEmpty", "The topic name cannot be empty.");

        public static readonly Func<int, Error> TooLong = 
            length => Validation("TopicName.TooLong", $"The topic name cannot exceed {length} characters.");

        public static readonly Func<int, Error> TooShort = 
            length => Validation("TopicName.TooShort", $"The topic name must be at least {length} characters.");
    }
}
