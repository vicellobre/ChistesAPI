using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Emails;

public partial class EmailTests
{
    public class ImplicitConversion
    {
        [Fact]
        public void Should_ReturnValue()
        {
            // Arrange
            Email email = Email.Create("user@example.com").Value;

            // Act
            string result = email;

            // Assert
            result.Should().Be("user@example.com");
        }
    }
}
