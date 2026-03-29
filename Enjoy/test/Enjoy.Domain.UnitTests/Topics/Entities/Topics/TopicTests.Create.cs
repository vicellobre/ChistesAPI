using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Topics.Entities;
using Enjoy.Domain.Topics.Events;

namespace Enjoy.Domain.UnitTests.Topics.Entities.Topics;

public partial class TopicTests
{
    public class Create
    {
        private const string ValidName = "Programming";

        [Fact]
        public void Should_ReturnSuccess_When_DataIsValid()
        {
            // Arrange & Act
            var result = Topic.Create(ValidName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().StartWith("topic_");
            result.Value.Name.Value.Should().Be("Programming");
        }

        [Fact]
        public void Should_RaiseTopicCreatedDomainEvent_When_DataIsValid()
        {
            // Arrange & Act
            var result = Topic.Create(ValidName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var events = result.Value.GetDomainEvents();
            events.Should().ContainSingle();
            var evt = (TopicCreatedDomainEvent)events.Single();
            evt.TopicId.Should().Be(result.Value.Id);
        }

        [Fact]
        public void Should_ReturnFailure_When_NameIsInvalid()
        {
            // Arrange & Act
            var result = Topic.Create("");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.TopicName.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnFailure_When_NameIsTooShort()
        {
            // Arrange & Act
            var result = Topic.Create("A");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.TopicName.TooShort(2));
        }
    }
}
