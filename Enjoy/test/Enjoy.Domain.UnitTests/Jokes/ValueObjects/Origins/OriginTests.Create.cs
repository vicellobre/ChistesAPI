using Enjoy.Domain.Jokes.ValueObjects;
using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Jokes.ValueObjects.Origins;

public partial class OriginTests
{
    public class Create
    {
        [Theory]
        [InlineData("ChuckNorris")]
        [InlineData("DadJoke")]
        [InlineData("Local")]
        [InlineData("chucknorris")]
        [InlineData("DADJOKE")]
        [InlineData("local")]
        public void Should_ReturnSuccess_When_OriginIsValid(string origin)
        {
            // Arrange & Act
            var result = Origin.Create(origin);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Should_NormalizeToCanonical_When_OriginIsValid()
        {
            // Arrange & Act
            var result = Origin.Create("chucknorris");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("ChuckNorris");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnIsNullOrEmptyError_When_NullOrWhitespace(string? value)
        {
            // Arrange & Act
            var result = Origin.Create(value);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.Origin.IsNullOrEmpty);
        }

        [Theory]
        [InlineData("Reddit")]
        [InlineData("Twitter")]
        [InlineData("Unknown")]
        public void Should_ReturnInvalidError_When_OriginIsInvalid(string value)
        {
            // Arrange & Act
            var result = Origin.Create(value);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.Origin.Invalid);
        }
    }
}
