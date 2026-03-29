using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Roles;

public partial class RoleTests
{
    public class Create
    {
        [Theory]
        [InlineData("User")]
        [InlineData("Admin")]
        [InlineData("user")]
        [InlineData("ADMIN")]
        public void Should_ReturnSuccess_When_RoleIsValid(string role)
        {
            // Arrange & Act
            var result = Role.Create(role);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Should_NormalizeToCanonical_When_RoleIsValid()
        {
            // Arrange & Act
            var result = Role.Create("admin");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Admin");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnIsNullOrEmptyError_When_NullOrWhitespace(string? value)
        {
            // Arrange & Act
            var result = Role.Create(value);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.Role.IsNullOrEmpty);
        }

        [Theory]
        [InlineData("SuperAdmin")]
        [InlineData("Guest")]
        [InlineData("Moderator")]
        public void Should_ReturnInvalidError_When_RoleIsInvalid(string value)
        {
            // Arrange & Act
            var result = Role.Create(value);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.Role.Invalid);
        }
    }
}
