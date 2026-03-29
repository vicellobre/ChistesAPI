using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Roles;

public partial class RoleTests
{
    public class Constants
    {
        [Fact]
        public void User_Should_BeUser() =>
            Role.User.Should().Be("User");

        [Fact]
        public void Admin_Should_BeAdmin() =>
            Role.Admin.Should().Be("Admin");

        [Fact]
        public void All_Should_ContainUserAndAdmin()
        {
            // Arrange & Act & Assert
            Role.All.Should().HaveCount(2);
            Role.All.Should().Contain("User");
            Role.All.Should().Contain("Admin");
        }
    }
}
