using Enjoy.Domain.Jokes.ValueObjects;

namespace Enjoy.Domain.UnitTests.Jokes.ValueObjects.Origins;

public partial class OriginTests
{
    public class Factories
    {
        [Fact]
        public void Should_ReturnChuckNorrisOrigin_When_FromChuckNorris()
        {
            // Arrange & Act
            var origin = Origin.FromChuckNorris();

            // Assert
            origin.Value.Should().Be("ChuckNorris");
        }

        [Fact]
        public void Should_ReturnDadJokeOrigin_When_FromDadJoke()
        {
            // Arrange & Act
            var origin = Origin.FromDadJoke();

            // Assert
            origin.Value.Should().Be("DadJoke");
        }

        [Fact]
        public void Should_ReturnLocalOrigin_When_FromLocal()
        {
            // Arrange & Act
            var origin = Origin.FromLocal();

            // Assert
            origin.Value.Should().Be("Local");
        }
    }
}
