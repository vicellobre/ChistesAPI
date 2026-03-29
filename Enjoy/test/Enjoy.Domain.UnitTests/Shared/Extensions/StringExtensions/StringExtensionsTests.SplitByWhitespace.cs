using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.StringExtensions;

public partial class StringExtensionsTests
{
    public class SplitByWhitespace
    {
        [Fact]
        public void Should_SplitCorrectly_When_MultipleWords()
        {
            // Arrange & Act
            var result = "hello world now".SplitByWhitespace();

            // Assert
            result.Should().BeEquivalentTo("hello", "world", "now");
        }

        [Fact]
        public void Should_SplitCorrectly_When_MultipleSpaces()
        {
            // Arrange & Act
            var result = "hello   world".SplitByWhitespace();

            // Assert
            result.Should().BeEquivalentTo("hello", "world");
        }

        [Fact]
        public void Should_SplitCorrectly_When_TabsAndNewlines()
        {
            // Arrange & Act
            var result = "hello\tworld\nnow".SplitByWhitespace();

            // Assert
            result.Should().BeEquivalentTo("hello", "world", "now");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_ReturnEmptyArray_When_NullOrEmpty(string? input)
        {
            // Arrange & Act
            var result = input!.SplitByWhitespace();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnSingleElement_When_SingleWord()
        {
            // Arrange & Act
            var result = "hello".SplitByWhitespace();

            // Assert
            result.Should().ContainSingle().Which.Should().Be("hello");
        }
    }
}
