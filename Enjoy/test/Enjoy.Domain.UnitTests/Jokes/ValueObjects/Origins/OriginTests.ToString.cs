using Enjoy.Domain.Jokes.ValueObjects;

namespace Enjoy.Domain.UnitTests.Jokes.ValueObjects.Origins;

public partial class OriginTests
{
    public class ToStringMethod
    {
        [Fact]
        public void Should_ReturnOriginValue()
        {
            // Arrange
            var origin = Origin.FromChuckNorris();

            // Act
            var result = origin.ToString();

            // Assert
            result.Should().Be("ChuckNorris");
        }
    }

    public class ImplicitConversion
    {
        [Fact]
        public void Should_ReturnStringValue()
        {
            // Arrange
            var origin = Origin.FromDadJoke();

            // Act
            string result = origin;

            // Assert
            result.Should().Be("DadJoke");
        }
    }
}
