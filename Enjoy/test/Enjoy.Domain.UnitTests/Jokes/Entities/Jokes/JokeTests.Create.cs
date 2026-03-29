using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Jokes.Events;
using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Jokes.Entities.Jokes;

public partial class JokeTests
{
    public class Create
    {
        private const string ValidText = "Why did the chicken cross the road?";
        private const string ValidAuthorId = "usr_author";
        private const string ValidOrigin = "Local";

        [Fact]
        public void Should_ReturnSuccess_When_DataIsValid()
        {
            // Arrange & Act
            var result = Joke.Create(ValidText, ValidAuthorId, ValidOrigin);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().StartWith("joke_");
            result.Value.Text.Value.Should().Be(ValidText);
            result.Value.AuthorId.Should().Be(ValidAuthorId);
            result.Value.Origin.Value.Should().Be("Local");
            result.Value.TopicIds.Should().BeEmpty();
        }

        [Fact]
        public void Should_RaiseJokeCreatedDomainEvent_When_DataIsValid()
        {
            // Arrange & Act
            var result = Joke.Create(ValidText, ValidAuthorId, ValidOrigin);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var events = result.Value.GetDomainEvents();
            events.Should().ContainSingle();
            var evt = (JokeCreatedDomainEvent)events.Single();
            evt.JokeId.Should().Be(result.Value.Id);
            evt.AuthorId.Should().Be(ValidAuthorId);
        }

        [Fact]
        public void Should_TrimAuthorId_When_DataIsValid()
        {
            // Arrange & Act
            var result = Joke.Create(ValidText, "  usr_author  ", ValidOrigin);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.AuthorId.Should().Be("usr_author");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnAuthorRequiredError_When_AuthorIdIsEmpty(string? authorId)
        {
            // Arrange & Act
            var result = Joke.Create(ValidText, authorId!, ValidOrigin);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Joke.AuthorRequired);
        }

        [Fact]
        public void Should_ReturnFailure_When_TextIsInvalid()
        {
            // Arrange & Act
            var result = Joke.Create("", ValidAuthorId, ValidOrigin);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.JokeText.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnFailure_When_OriginIsInvalid()
        {
            // Arrange & Act
            var result = Joke.Create(ValidText, ValidAuthorId, "Invalid");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Origin.Invalid);
        }

        [Fact]
        public void Should_ReturnAllErrors_When_MultipleFieldsAreInvalid()
        {
            // Arrange & Act
            var result = Joke.Create("", "", "Invalid");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().HaveCountGreaterThanOrEqualTo(3);
        }
    }
}
