using Enjoy.Domain.Shared.Primitives;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Jokes.Entities;

namespace Enjoy.Domain.UnitTests.Shared.Primitives.Entities;

public partial class EntityTests
{
    public class EqualOperator
    {
        [Fact]
        public void Should_ReturnTrue_When_SameReference()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var same = user;

            // Act
            var result = user == same;
                
            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NullFirst()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act
            var result = null! == user;

            // Assert
            result.Should().BeFalse();
        }
    }

    public class NotEqualOperator
    {
        [Fact]
        public void Should_ReturnTrue_When_DifferentEntities()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var joke = Joke.Create("A joke", "usr_1", "Local").Value;

            // Act
            var result = (Entity)user != (Entity)joke;
            
            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_SameReference()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var same = user;

            // Act
            var result = user != same;

            // Assert
            result.Should().BeFalse();
        }
    }
}
