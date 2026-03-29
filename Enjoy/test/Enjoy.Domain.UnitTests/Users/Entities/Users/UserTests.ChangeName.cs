using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Events;

namespace Enjoy.Domain.UnitTests.Users.Entities.Users;

public partial class UserTests
{
    public class ChangeName
    {
        private static User CreateValidUser() =>
            User.Create("Vicente", "user@example.com", "$2a$10$hash", "User").Value;

        [Fact]
        public void Should_UpdateName_When_NameIsValid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeName("Carlos");

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.Name.Value.Should().Be("Carlos");
        }

        [Fact]
        public void Should_RaiseUserNameChangedDomainEvent_When_NameIsValid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            user.ChangeName("Carlos");

            // Assert
            var events = user.GetDomainEvents();
            events.Should().ContainSingle();
            events.Single().Should().BeOfType<UserNameChangedDomainEvent>();
        }

        [Fact]
        public void Should_NotRaiseEvent_When_SameName()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeName("Vicente");

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnFailure_When_NameIsInvalid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeName("");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Name.IsNullOrEmpty);
            user.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_NotMutateState_When_NameIsInvalid()
        {
            // Arrange
            var user = CreateValidUser();
            var originalName = user.Name;

            // Act
            user.ChangeName("");

            // Assert
            user.Name.Should().Be(originalName);
        }
    }
}
