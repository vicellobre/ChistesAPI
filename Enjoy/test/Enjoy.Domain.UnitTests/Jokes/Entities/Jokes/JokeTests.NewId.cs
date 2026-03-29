using Enjoy.Domain.Jokes.Entities;

namespace Enjoy.Domain.UnitTests.Jokes.Entities.Jokes;

public partial class JokeTests
{
    public class NewId
    {
        [Fact]
        public void Should_StartWithPrefix()
        {
            // Arrange & Act
            var id = Joke.NewId();

            // Assert
            id.Should().StartWith("joke_");
        }

        [Fact]
        public void Should_GenerateUniqueIds()
        {
            // Arrange & Act
            var id1 = Joke.NewId();
            var id2 = Joke.NewId();

            // Assert
            id1.Should().NotBe(id2);
        }
    }
}
