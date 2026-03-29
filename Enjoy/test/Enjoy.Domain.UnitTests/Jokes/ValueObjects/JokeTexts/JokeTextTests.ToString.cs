using Enjoy.Domain.Jokes.ValueObjects;

namespace Enjoy.Domain.UnitTests.Jokes.ValueObjects.JokeTexts;

public partial class JokeTextTests
{
    public class ToStringMethod
    {
        [Fact]
        public void Should_ReturnTextValue()
        {
            // Arrange
            var jokeText = JokeText.Create("A funny joke").Value;

            // Act
            var result = jokeText.ToString();

            // Assert
            result.Should().Be("A funny joke");
        }
    }

    public class ImplicitConversion
    {
        [Fact]
        public void Should_ReturnStringValue()
        {
            // Arrange
            var jokeText = JokeText.Create("A funny joke").Value;

            // Act
            string result = jokeText;

            // Assert
            result.Should().Be("A funny joke");
        }
    }
}
