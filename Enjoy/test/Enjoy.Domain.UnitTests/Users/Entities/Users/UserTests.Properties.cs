using System.Reflection;
using Enjoy.Domain.Users.Entities;

namespace Enjoy.Domain.UnitTests.Users.Entities.Users;

public partial class UserTests
{
    public class Properties
    {
        [Fact]
        public void Should_BeDefault_When_CreatedOnUtcAfterCreate()
        {
            // Arrange & Act
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Assert
            user.CreatedOnUtc.Should().Be(default);
        }

        [Fact]
        public void Should_BeNull_When_ModifiedOnUtcAfterCreate()
        {
            // Arrange & Act
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Assert
            user.ModifiedOnUtc.Should().BeNull();
        }

        [Fact]
        public void Should_SetValue_When_CreatedOnUtcAssigned()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var now = DateTime.UtcNow;

            // Act
            user.CreatedOnUtc = now;

            // Assert
            user.CreatedOnUtc.Should().Be(now);
        }

        [Fact]
        public void Should_SetValue_When_ModifiedOnUtcAssigned()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;
            var now = DateTime.UtcNow;

            // Act
            user.ModifiedOnUtc = now;

            // Assert
            user.ModifiedOnUtc.Should().Be(now);
        }
    }

    public class ParameterlessConstructor
    {
        [Fact]
        public void Should_CreateInstance_When_UsingReflection()
        {
            // Arrange
            var ctor = typeof(User).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null, types: Type.EmptyTypes, modifiers: null);

            // Act
            var user = (User)ctor!.Invoke(null);

            // Assert
            user.Should().NotBeNull();
        }
    }
}
