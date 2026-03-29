using Enjoy.Domain.Users.Entities;

namespace Enjoy.Domain.UnitTests.Users.Entities.Users;

public partial class UserTests
{
    public class NewId
    {
        [Fact]
        public void Should_StartWithPrefix()
        {
            // Arrange & Act
            var id = User.NewId();

            // Assert
            id.Should().StartWith("usr_");
        }

        [Fact]
        public void Should_GenerateUniqueIds()
        {
            // Arrange & Act
            var id1 = User.NewId();
            var id2 = User.NewId();

            // Assert
            id1.Should().NotBe(id2);
        }
    }
}
