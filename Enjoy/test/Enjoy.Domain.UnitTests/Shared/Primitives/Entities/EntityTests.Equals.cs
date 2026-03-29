using System.Reflection;
using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Shared.Primitives;
using Enjoy.Domain.Users.Entities;

namespace Enjoy.Domain.UnitTests.Shared.Primitives.Entities;

public partial class EntityTests
{
    public class EqualsEntity
    {
        [Fact]
        public void Should_ReturnFalse_When_Null()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act & Assert
            user.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_DifferentTypesWithSameId()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var joke = Joke.Create("A joke", "usr_1", "Local").Value;

            // set id of joke to be the same as user using reflection
            var jokeIdProp = typeof(Entity).GetProperty("Id")!;
            jokeIdProp.SetValue(joke, user.Id);

            // Act & Assert
            user.Equals((Entity)joke).Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_SameReference()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act & Assert
            user.Equals((Entity?)user).Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_SameId()
        {
            // Arrange
            var user1 = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var user2 = User.Create("Other", "other@example.com", "$2a$10$hash", "User").Value;

            var userIdProp = typeof(Entity).GetProperty("Id")!;
            userIdProp.SetValue(user2, user1.Id);

            // Act & Assert
            user1.Equals(user2).Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_DifferentId()
        {
            // Arrange
            var user1 = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var user2 = User.Create("Other", "other@example.com", "$2a$10$hash", "User").Value;

            // Act
            var result = user1.Equals((Entity?)user2);

            // Assert
            Assert.False(result);
        }
    }

    public class EqualsObject
    {
        [Fact]
        public void Should_ReturnFalse_When_Null()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act & Assert
            user.Equals((object?)null).Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_SameReference()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act
            var result = user.Equals((object)user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Should_ReturnFalse_When_DifferentType()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act & Assert
            user.Equals("not an entity").Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_DifferentId()
        {
            // Arrange
            var user1 = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var user2 = User.Create("Other", "other@example.com", "$2a$10$hash", "User").Value;

            // Act
            var result = user1.Equals((object)user2);

            // Assert
            Assert.False(result);
        }
    }
}
