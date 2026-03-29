using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Topics.Entities;

namespace Enjoy.Domain.UnitTests.Jokes.Entities.Jokes;

public partial class JokeTests
{
    public class RemoveTopic
    {
        private static Joke CreateJokeWithTopics()
        {
            var joke = Joke.Create("A funny joke", "usr_author", "Local").Value;
            var topic1 = Topic.Create("Tech").Value;
            var topic2 = Topic.Create("Sport").Value;
            joke.AddTopic(topic1);
            joke.AddTopic(topic2);
            return joke;
        }

        [Fact]
        public void Should_RemoveFromCollection_When_TopicIdExists()
        {
            // Arrange
            var joke = CreateJokeWithTopics();
            var topicToRemove = joke.Topics.First();

            // Act
            var result = joke.RemoveTopic(topicToRemove.Id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            joke.TopicIds.Should().ContainSingle();
            joke.TopicIds.Should().NotContain(topicToRemove.Id);
        }

        [Fact]
        public void Should_ReturnSuccess_When_TopicIdDoesNotExist()
        {
            // Arrange
            var joke = CreateJokeWithTopics();

            // Act
            var result = joke.RemoveTopic("topic_999");

            // Assert
            result.IsSuccess.Should().BeTrue();
            joke.TopicIds.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnSuccess_When_NullOrWhitespace(string? topicId)
        {
            // Arrange
            var joke = CreateJokeWithTopics();

            // Act
            var result = joke.RemoveTopic(topicId!);

            // Assert
            result.IsSuccess.Should().BeTrue();
            joke.TopicIds.Should().HaveCount(2);
        }
    }
}
