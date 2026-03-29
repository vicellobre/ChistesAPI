using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Topics.Entities;

namespace Enjoy.Domain.UnitTests.Jokes.Entities.Jokes;

public partial class JokeTests
{
    public class AddTopic
    {
        private static Joke CreateValidJoke() =>
            Joke.Create("A funny joke", "usr_author", "Local").Value;

        [Fact]
        public void Should_AddToCollection_When_TopicIsValid()
        {
            // Arrange
            var joke = CreateValidJoke();
            var topic = Topic.Create("Tech").Value;

            // Act
            var result = joke.AddTopic(topic);

            // Assert
            result.IsSuccess.Should().BeTrue();
            joke.TopicIds.Should().ContainSingle().Which.Should().Be(topic.Id);
        }

        [Fact]
        public void Should_AddAll_When_TopicsAreDifferent()
        {
            // Arrange
            var joke = CreateValidJoke();
            var topic1 = Topic.Create("Tech").Value;
            var topic2 = Topic.Create("Sport").Value;

            // Act
            joke.AddTopic(topic1);
            joke.AddTopic(topic2);

            // Assert
            joke.TopicIds.Should().HaveCount(2);
            joke.TopicIds.Should().Contain(topic1.Id);
            joke.TopicIds.Should().Contain(topic2.Id);
        }

        [Fact]
        public void Should_NotAddAgain_When_TopicIsDuplicate()
        {
            // Arrange
            var joke = CreateValidJoke();
            var topic = Topic.Create("Tech").Value;
            joke.AddTopic(topic);

            // Act
            var result = joke.AddTopic(topic);

            // Assert
            result.IsSuccess.Should().BeTrue();
            joke.TopicIds.Should().ContainSingle();
        }

        [Fact]
        public void Should_ReturnTopicIdRequiredError_When_TopicIsNull()
        {
            // Arrange
            var joke = CreateValidJoke();

            // Act
            var result = joke.AddTopic(null!);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.Joke.TopicIdRequired);
        }
    }
}
