using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.StringExtensions;

public partial class StringExtensionsTests
{
    public class CapitalizeWords
    {
        [Theory]
        [InlineData("hello world", "Hello World")]
        [InlineData("HELLO WORLD", "Hello World")]
        [InlineData("hElLo wOrLd", "Hello World")]
        public void Should_CapitalizeEachWord_When_ValidInput(string input, string expected)
        {
            // Arrange & Act
            var result = input.CapitalizeWords();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Should_CapitalizeIt_When_SingleWord()
        {
            // Arrange & Act
            var result = "hello".CapitalizeWords();

            // Assert
            result.Should().Be("Hello");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnInput_When_NullOrWhitespace(string? input)
        {
            // Arrange & Act
            var result = input!.CapitalizeWords();

            // Assert
            result.Should().Be(input);
        }

        [Fact]
        public void Should_CollapseToSingleSpace_When_MultipleSpaces()
        {
            // Arrange & Act
            var result = "hello    world".CapitalizeWords();

            // Assert
            result.Should().Be("Hello World");
        }
    }
}
