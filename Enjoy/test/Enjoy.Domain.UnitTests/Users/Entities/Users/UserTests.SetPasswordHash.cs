using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Events;

namespace Enjoy.Domain.UnitTests.Users.Entities.Users;

public partial class UserTests
{
    public class SetPasswordHash
    {
        private static User CreateValidUser() =>
            User.Create("Vicente", "user@example.com", "$2a$10$hash", "User").Value;

        [Fact]
        public void Should_UpdatePasswordHash_When_HashIsValid()
        {
            // Arrange
            var user = CreateValidUser();

            // Act
            var result = user.SetPasswordHash("$2a$10$newhash");

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.PasswordHash.Value.Should().Be("$2a$10$newhash");
        }

        [Fact]
        public void Should_RaiseUserPasswordChangedDomainEvent_When_HashIsValid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            user.SetPasswordHash("$2a$10$newhash");

            // Assert
            var events = user.GetDomainEvents();
            events.Should().ContainSingle();
            events.Single().Should().BeOfType<UserPasswordChangedDomainEvent>();
        }

        [Fact]
        public void Should_NotRaiseEvent_When_SameHash()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.SetPasswordHash("$2a$10$hash");

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnFailure_When_HashIsInvalid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.SetPasswordHash("");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.PasswordHash.IsNullOrEmpty);
            user.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_NotMutateState_When_HashIsInvalid()
        {
            // Arrange
            var user = CreateValidUser();
            var originalHash = user.PasswordHash;

            // Act
            user.SetPasswordHash("");

            // Assert
            user.PasswordHash.Should().Be(originalHash);
        }
    }
}
