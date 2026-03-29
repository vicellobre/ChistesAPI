using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.StringExtensions;

public partial class StringExtensionsTests
{
    public class NormalizeWhitespace
    {
        [Fact]
        public void Should_CollapseToSingle_When_MultipleSpaces()
        {
            // Arrange & Act
            var result = "hello    world".NormalizeWhitespace();

            // Assert
            result.Should().Be("hello world");
        }

        [Fact]
        public void Should_Trim_When_LeadingAndTrailing()
        {
            // Arrange & Act
            var result = "  hello world  ".NormalizeWhitespace();

            // Assert
            result.Should().Be("hello world");
        }

        [Fact]
        public void Should_NormalizeToSpaces_When_TabsAndNewlines()
        {
            // Arrange & Act
            var result = "hello\t\tworld\nnow".NormalizeWhitespace();

            // Assert
            result.Should().Be("hello world now");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_ReturnInput_When_NullOrEmpty(string? input)
        {
            // Arrange & Act
            var result = input!.NormalizeWhitespace();

            // Assert
            result.Should().Be(input);
        }

        [Fact]
        public void Should_ReturnSame_When_AlreadyNormalized()
        {
            // Arrange
            const string input = "hello world";

            // Act
            var result = input.NormalizeWhitespace();

            // Assert
            result.Should().Be(input);
        }
    }
}
