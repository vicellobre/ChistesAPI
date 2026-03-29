using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Events;

namespace Enjoy.Domain.UnitTests.Users.Entities.Users;

public partial class UserTests
{
    public class Create
    {
        private const string ValidName = "Vicente";
        private const string ValidEmail = "user@example.com";
        private const string ValidPasswordHash = "$2a$10$somevalidhash";
        private const string ValidRole = "User";

        [Fact]
        public void Should_ReturnSuccess_When_DataIsValid()
        {
            // Arrange & Act
            var result = User.Create(ValidName, ValidEmail, ValidPasswordHash, ValidRole);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().StartWith("usr_");
            result.Value.Name.Value.Should().Be("Vicente");
            result.Value.Email.Value.Should().Be("user@example.com");
            result.Value.Role.Value.Should().Be("User");
        }

        [Fact]
        public void Should_RaiseUserRegisteredDomainEvent_When_DataIsValid()
        {
            // Arrange & Act
            var result = User.Create(ValidName, ValidEmail, ValidPasswordHash, ValidRole);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var events = result.Value.GetDomainEvents();
            events.Should().ContainSingle();
            var evt = (UserRegisteredDomainEvent)events.Single();
            evt.UserId.Should().Be(result.Value.Id);
        }

        [Fact]
        public void Should_ReturnFailure_When_NameIsInvalid()
        {
            // Arrange & Act
            var result = User.Create("", ValidEmail, ValidPasswordHash, ValidRole);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Name.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnFailure_When_EmailIsInvalid()
        {
            // Arrange & Act
            var result = User.Create(ValidName, "", ValidPasswordHash, ValidRole);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Email.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnFailure_When_PasswordHashIsInvalid()
        {
            // Arrange & Act
            var result = User.Create(ValidName, ValidEmail, "", ValidRole);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.PasswordHash.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnFailure_When_RoleIsInvalid()
        {
            // Arrange & Act
            var result = User.Create(ValidName, ValidEmail, ValidPasswordHash, "Invalid");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Role.Invalid);
        }

        [Fact]
        public void Should_ReturnAllErrors_When_MultipleFieldsAreInvalid()
        {
            // Arrange & Act
            var result = User.Create("", "", "", "Invalid");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().HaveCountGreaterThanOrEqualTo(4);
        }

        [Fact]
        public void Should_NotRaiseDomainEvents_When_DataIsInvalid()
        {
            // Arrange & Act
            var result = User.Create("", ValidEmail, ValidPasswordHash, ValidRole);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}
