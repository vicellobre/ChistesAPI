using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Jokes.Events;
using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Jokes.Entities.Jokes;

public partial class JokeTests
{
    public class UpdateText
    {
        private static Joke CreateValidJoke() =>
            Joke.Create("Original joke text", "usr_author", "Local").Value;

        [Fact]
        public void Should_UpdateText_When_TextIsValid()
        {
            // Arrange
            var joke = CreateValidJoke();

            // Act
            var result = joke.UpdateText("New joke text");

            // Assert
            result.IsSuccess.Should().BeTrue();
            joke.Text.Value.Should().Be("New joke text");
        }

        [Fact]
        public void Should_RaiseJokeTextUpdatedDomainEvent_When_TextIsValid()
        {
            // Arrange
            var joke = CreateValidJoke();
            joke.ClearDomainEvents();

            // Act
            joke.UpdateText("New joke text");

            // Assert
            var events = joke.GetDomainEvents();
            events.Should().ContainSingle();
            events.Single().Should().BeOfType<JokeTextUpdatedDomainEvent>();
        }

        [Fact]
        public void Should_NotRaiseEvent_When_TextIsSame()
        {
            // Arrange
            var joke = CreateValidJoke();
            joke.ClearDomainEvents();

            // Act
            var result = joke.UpdateText("Original joke text");

            // Assert
            result.IsSuccess.Should().BeTrue();
            joke.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnFailure_When_TextIsInvalid()
        {
            // Arrange
            var joke = CreateValidJoke();
            joke.ClearDomainEvents();

            // Act
            var result = joke.UpdateText("");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.JokeText.IsNullOrEmpty);
            joke.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_NotMutateState_When_TextIsInvalid()
        {
            // Arrange
            var joke = CreateValidJoke();
            var originalText = joke.Text;

            // Act
            joke.UpdateText("");

            // Assert
            joke.Text.Should().Be(originalText);
        }
    }
}
