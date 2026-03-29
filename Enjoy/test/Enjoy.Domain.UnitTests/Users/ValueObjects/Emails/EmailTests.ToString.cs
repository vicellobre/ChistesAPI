using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Emails;

public partial class EmailTests
{
    public class ToStringMethod
    {
        [Fact]
        public void Should_ReturnEmailValue()
        {
            // Arrange
            var email = Email.Create("user@example.com").Value;

            // Act
            var result = email.ToString();

            // Assert
            result.Should().Be("user@example.com");
        }
    }
}
