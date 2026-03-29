using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.StringExtensions;

public partial class StringExtensionsTests
{
    public class Capitalize
    {
        [Theory]
        [InlineData("hello", "Hello")]
        [InlineData("HELLO", "Hello")]
        [InlineData("hElLo", "Hello")]
        public void Should_CapitalizeCorrectly_When_ValidInput(string input, string expected)
        {
            // Arrange & Act
            var result = input.Capitalize();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnUppercase_When_SingleChar()
        {
            // Arrange & Act
            var result = "a".Capitalize();

            // Assert
            result.Should().Be("A");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnInput_When_NullOrWhitespace(string? input)
        {
            // Arrange & Act
            var result = input!.Capitalize();

            // Assert
            result.Should().Be(input);
        }
    }
}
