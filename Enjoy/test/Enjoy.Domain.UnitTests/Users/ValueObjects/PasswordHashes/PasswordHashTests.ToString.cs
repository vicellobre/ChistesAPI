using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.PasswordHashes;

public partial class PasswordHashTests
{
    public class ImplicitConversion
    {
        [Fact]
        public void Should_ReturnValue()
        {
            // Arrange
            var hash = PasswordHash.Create("$2a$10$somevalidhash").Value;

            // Act
            string result = hash;

            // Assert
            result.Should().Be("$2a$10$somevalidhash");
        }
    }

    public class ToStringMethod
    {
        [Fact]
        public void Should_ReturnHashValue()
        {
            // Arrange
            var hash = PasswordHash.Create("$2a$10$somevalidhash").Value;

            // Act
            var result = hash.ToString();

            // Assert
            result.Should().Be("$2a$10$somevalidhash");
        }
    }
}
