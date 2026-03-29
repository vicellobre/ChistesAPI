using Enjoy.Domain.Jokes.ValueObjects;
using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Jokes.ValueObjects.JokeTexts;

public partial class JokeTextTests
{
    public class Create
    {
        [Fact]
        public void Should_ReturnSuccess_When_TextIsValid()
        {
            // Arrange
            const string text = "Why did the chicken cross the road?";

            // Act
            var result = JokeText.Create(text);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(text);
        }

        [Fact]
        public void Should_NormalizeWhitespace_When_TextIsValid()
        {
            // Arrange
            const string text = "  Why   did   the   chicken  cross? ";

            // Act
            var result = JokeText.Create(text);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Why did the chicken cross?");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnIsNullOrEmptyError_When_NullOrWhitespace(string? value)
        {
            // Arrange & Act
            var result = JokeText.Create(value);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.JokeText.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnTooLongError_When_TooLong()
        {
            // Arrange
            var text = new string('a', JokeText.MaxLength + 1);

            // Act
            var result = JokeText.Create(text);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.JokeText.TooLong(JokeText.MaxLength));
        }

        [Fact]
        public void Should_ReturnSuccess_When_ExactMaxLength()
        {
            // Arrange
            var text = new string('a', JokeText.MaxLength);

            // Act
            var result = JokeText.Create(text);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
