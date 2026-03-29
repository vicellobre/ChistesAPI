using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Events;

namespace Enjoy.Domain.UnitTests.Users.Entities.Users;

public partial class UserTests
{
    public class ChangeEmail
    {
        private static User CreateValidUser() =>
            User.Create("Vicente", "user@example.com", "$2a$10$hash", "User").Value;

        [Fact]
        public void Should_UpdateEmail_When_EmailIsValid()
        {
            // Arrange
            var user = CreateValidUser();

            // Act
            var result = user.ChangeEmail("new@example.com");

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.Email.Value.Should().Be("new@example.com");
        }

        [Fact]
        public void Should_RaiseUserEmailChangedDomainEvent_When_EmailIsValid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            user.ChangeEmail("new@example.com");

            // Assert
            var events = user.GetDomainEvents();
            events.Should().ContainSingle();
            events.Single().Should().BeOfType<UserEmailChangedDomainEvent>();
        }

        [Fact]
        public void Should_NotRaiseEvent_When_SameEmail()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeEmail("user@example.com");

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnFailure_When_EmailIsInvalid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeEmail("");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Email.IsNullOrEmpty);
            user.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_NotMutateState_When_EmailIsInvalid()
        {
            // Arrange
            var user = CreateValidUser();
            var originalEmail = user.Email;

            // Act
            var result = user.ChangeEmail("");

            // Assert
            result.IsFailure.Should().BeTrue();
            user.Email.Should().Be(originalEmail);
        }
    }
}
