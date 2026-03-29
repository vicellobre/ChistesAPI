using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Names;

public partial class NameTests
{
    public class ToStringMethod
    {
        [Fact]
        public void Should_ReturnNameValue()
        {
            // Arrange
            var name = Name.Create("Vicente").Value;

            // Act
            var result = name.ToString();

            // Assert
            result.Should().Be("Vicente");
        }
    }
}
