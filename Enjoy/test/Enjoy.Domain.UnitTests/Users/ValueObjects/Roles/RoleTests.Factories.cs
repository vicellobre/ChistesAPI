using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Roles;

public partial class RoleTests
{
    public class Factories
    {
        [Fact]
        public void Should_ReturnUserRole_When_CreateUser()
        {
            // Arrange & Act
            var role = Role.CreateUser();

            // Assert
            role.Value.Should().Be("User");
        }

        [Fact]
        public void Should_ReturnAdminRole_When_CreateAdmin()
        {
            // Arrange & Act
            var role = Role.CreateAdmin();

            // Assert
            role.Value.Should().Be("Admin");
        }
    }
}
