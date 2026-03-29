using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Events;

namespace Enjoy.Domain.UnitTests.Users.Entities.Users;

public partial class UserTests
{
    public class ChangeRole
    {
        private static User CreateValidUser() =>
            User.Create("Vicente", "user@example.com", "$2a$10$hash", "User").Value;

        [Fact]
        public void Should_UpdateRole_When_RoleIsValid()
        {
            // Arrange
            var user = CreateValidUser();

            // Act
            var result = user.ChangeRole("Admin");

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.Role.Value.Should().Be("Admin");
        }

        [Fact]
        public void Should_RaiseUserRoleChangedDomainEvent_When_RoleIsValid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            user.ChangeRole("Admin");

            // Assert
            var events = user.GetDomainEvents();
            events.Should().ContainSingle();
            var evt = (UserRoleChangedDomainEvent)events.Single();
            evt.UserId.Should().Be(user.Id);
            evt.NewRole.Should().Be("Admin");
        }

        [Fact]
        public void Should_NotRaiseEvent_When_SameRole()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeRole("User");

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnFailure_When_RoleIsInvalid()
        {
            // Arrange
            var user = CreateValidUser();
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeRole("SuperAdmin");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.Role.Invalid);
            user.GetDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public void Should_NotMutateState_When_RoleIsInvalid()
        {
            // Arrange
            var user = CreateValidUser();
            var originalRole = user.Role;

            // Act
            user.ChangeRole("SuperAdmin");

            // Assert
            user.Role.Should().Be(originalRole);
        }
    }
}
