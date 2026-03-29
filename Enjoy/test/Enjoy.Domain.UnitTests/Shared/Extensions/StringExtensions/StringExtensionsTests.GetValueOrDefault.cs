using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.StringExtensions;

public partial class StringExtensionsTests
{
    public class GetValueOrDefault
    {
        [Fact]
        public void Should_ReturnInput_When_NonEmpty()
        {
            // Arrange & Act
            var result = "hello".GetValueOrDefault("default");

            // Assert
            result.Should().Be("hello");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_ReturnDefault_When_NullOrEmpty(string? input)
        {
            // Arrange & Act
            var result = input!.GetValueOrDefault("default");

            // Assert
            result.Should().Be("default");
        }
    }
}
