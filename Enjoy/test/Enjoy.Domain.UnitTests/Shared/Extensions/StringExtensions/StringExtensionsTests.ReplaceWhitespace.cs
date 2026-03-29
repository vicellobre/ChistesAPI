using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.StringExtensions;

public partial class StringExtensionsTests
{
    public class ReplaceWhitespace
    {
        [Fact]
        public void Should_ReplaceWithValue_When_MultipleSpaces()
        {
            // Arrange & Act
            var result = "hello   world".ReplaceWhitespace("-");

            // Assert
            result.Should().Be("hello-world");
        }

        [Fact]
        public void Should_ReplaceWithValue_When_TabsAndNewlines()
        {
            // Arrange & Act
            var result = "hello\t\tworld\nnow".ReplaceWhitespace("_");

            // Assert
            result.Should().Be("hello_world_now");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_ReturnInput_When_NullOrEmpty(string? input)
        {
            // Arrange & Act
            var result = input!.ReplaceWhitespace("-");

            // Assert
            result.Should().Be(input);
        }
    }
}
