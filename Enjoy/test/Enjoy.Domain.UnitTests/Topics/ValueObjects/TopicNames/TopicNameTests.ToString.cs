using Enjoy.Domain.Topics.ValueObjects;

namespace Enjoy.Domain.UnitTests.Topics.ValueObjects.TopicNames;

public partial class TopicNameTests
{
    public class ToStringMethod
    {
        [Fact]
        public void Should_ReturnTopicNameValue()
        {
            // Arrange
            var topicName = TopicName.Create("Science").Value;

            // Act
            var result = topicName.ToString();

            // Assert
            result.Should().Be("Science");
        }
    }

    public class ImplicitConversion
    {
        [Fact]
        public void Should_ReturnStringValue()
        {
            // Arrange
            var topicName = TopicName.Create("Science").Value;

            // Act
            string result = topicName;

            // Assert
            result.Should().Be("Science");
        }
    }
}
