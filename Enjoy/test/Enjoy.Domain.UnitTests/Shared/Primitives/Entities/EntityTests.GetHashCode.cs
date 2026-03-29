using Enjoy.Domain.Users.Entities;

namespace Enjoy.Domain.UnitTests.Shared.Primitives.Entities;

public partial class EntityTests
{
    public class GetHashCodeMethod
    {
        [Fact]
        public void Should_ReturnConsistentValue()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act
            var hash1 = user.GetHashCode();
            var hash2 = user.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }
    }
}
