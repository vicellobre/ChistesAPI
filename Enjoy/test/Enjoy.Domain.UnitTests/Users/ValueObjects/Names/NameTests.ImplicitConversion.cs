using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Names;

public partial class NameTests
{
    public class ImplicitConversion
    {
        [Fact]
        public void Should_ReturnValue()
        {
            // Arrange
            var name = Name.Create("Vicente").Value;

            // Act
            string result = name;

            // Assert
            result.Should().Be("Vicente");
        }
    }
}
