using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Topics.Entities;

namespace Enjoy.Domain.UnitTests.Topics.Entities.Topics;

public partial class TopicTests
{
    public class ChangeName
    {
        private static Topic CreateValidTopic() =>
            Topic.Create("Science").Value;

        [Fact]
        public void Should_UpdateName_When_NameIsValid()
        {
            // Arrange
            var topic = CreateValidTopic();

            // Act
            var result = topic.ChangeName("Mathematics");

            // Assert
            result.IsSuccess.Should().BeTrue();
            topic.Name.Value.Should().Be("Mathematics");
        }

        [Fact]
        public void Should_NotMutateState_When_NameIsSame()
        {
            // Arrange
            var topic = CreateValidTopic();

            // Act
            var result = topic.ChangeName("Science");

            // Assert
            result.IsSuccess.Should().BeTrue();
            topic.Name.Value.Should().Be("Science");
        }

        [Fact]
        public void Should_ReturnFailure_When_NameIsInvalid()
        {
            // Arrange
            var topic = CreateValidTopic();

            // Act
            var result = topic.ChangeName("");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.TopicName.IsNullOrEmpty);
        }

        [Fact]
        public void Should_NotMutateState_When_NameIsInvalid()
        {
            // Arrange
            var topic = CreateValidTopic();
            var originalName = topic.Name;

            // Act
            topic.ChangeName("");

            // Assert
            topic.Name.Should().Be(originalName);
        }
    }
}
