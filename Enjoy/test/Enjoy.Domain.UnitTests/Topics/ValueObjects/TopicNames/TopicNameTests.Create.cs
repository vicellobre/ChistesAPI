using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Topics.ValueObjects;

namespace Enjoy.Domain.UnitTests.Topics.ValueObjects.TopicNames;

public partial class TopicNameTests
{
    public class Create
    {
        [Fact]
        public void Should_ReturnSuccess_When_NameIsValid()
        {
            // Arrange
            const string name = "Programming";

            // Act
            var result = TopicName.Create(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Programming");
        }

        [Fact]
        public void Should_CapitalizeWords_When_NameIsValid()
        {
            // Arrange
            const string name = "dark humor";

            // Act
            var result = TopicName.Create(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Dark Humor");
        }

        [Fact]
        public void Should_TrimWhitespace_When_NameIsValid()
        {
            // Arrange
            const string name = "  Science  ";

            // Act
            var result = TopicName.Create(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Science");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnIsNullOrEmptyError_When_NullOrWhitespace(string? value)
        {
            // Arrange & Act
            var result = TopicName.Create(value);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.TopicName.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnTooShortError_When_TooShort()
        {
            // Arrange
            const string name = "A";

            // Act
            var result = TopicName.Create(name);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.TopicName.TooShort(TopicName.MinLength));
        }

        [Fact]
        public void Should_ReturnTooLongError_When_TooLong()
        {
            // Arrange
            var name = new string('A', TopicName.MaxLength + 1);

            // Act
            var result = TopicName.Create(name);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.TopicName.TooLong(TopicName.MaxLength));
        }
    }
}
