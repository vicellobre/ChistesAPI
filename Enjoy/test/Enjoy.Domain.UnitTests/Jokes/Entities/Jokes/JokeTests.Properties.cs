using System.Reflection;
using Enjoy.Domain.Jokes.Entities;

namespace Enjoy.Domain.UnitTests.Jokes.Entities.Jokes;

public partial class JokeTests
{
    public class Properties
    {
        [Fact]
        public void Should_BeDefault_When_CreatedOnUtcAfterCreate()
        {
            // Arrange & Act
            var joke = Joke.Create("A funny joke", "usr_1", "Local").Value;

            // Assert
            joke.CreatedOnUtc.Should().Be(default);
        }

        [Fact]
        public void Should_BeNull_When_ModifiedOnUtcAfterCreate()
        {
            // Arrange & Act
            var joke = Joke.Create("A funny joke", "usr_1", "Local").Value;

            // Assert
            joke.ModifiedOnUtc.Should().BeNull();
        }

        [Fact]
        public void Should_SetValue_When_CreatedOnUtcAssigned()
        {
            // Arrange
            var joke = Joke.Create("A funny joke", "usr_1", "Local").Value;
            var now = DateTime.UtcNow;

            // Act
            joke.CreatedOnUtc = now;

            // Assert
            joke.CreatedOnUtc.Should().Be(now);
        }

        [Fact]
        public void Should_SetValue_When_ModifiedOnUtcAssigned()
        {
            // Arrange
            var joke = Joke.Create("A funny joke", "usr_1", "Local").Value;
            var now = DateTime.UtcNow;

            // Act
            joke.ModifiedOnUtc = now;

            // Assert
            joke.ModifiedOnUtc.Should().Be(now);
        }
    }

    public class ParameterlessConstructor
    {
        [Fact]
        public void Should_CreateInstance_When_UsingReflection()
        {
            // Arrange
            var ctor = typeof(Joke).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null, types: Type.EmptyTypes, modifiers: null);

            // Act
            var joke = (Joke)ctor!.Invoke(null);

            // Assert
            joke.Should().NotBeNull();
            joke.TopicIds.Should().BeEmpty();
        }
    }
}
