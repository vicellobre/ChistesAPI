using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Roles;

public partial class RoleTests
{
    public class ToStringMethod
    {
        [Fact]
        public void Should_ReturnRoleValue()
        {
            // Arrange
            var role = Role.CreateUser();

            // Act
            var result = role.ToString();

            // Assert
            result.Should().Be("User");
        }
    }

    public class ImplicitConversion
    {
        [Fact]
        public void Should_ReturnStringValue()
        {
            // Arrange
            var role = Role.CreateAdmin();

            // Act
            string result = role;

            // Assert
            result.Should().Be("Admin");
        }
    }
}
