using Enjoy.Domain.Shared.Exceptions;

namespace Enjoy.Domain.UnitTests.Shared.Exceptions.DomainExceptions;

public class DomainExceptionTests
{
    private sealed class TestDomainException(string message) : DomainException(message);

    public class Constructor
    {
        [Fact]
        public void Should_SetMessage_When_MessageProvided()
        {
            // Arrange & Act
            var exception = new TestDomainException("Something went wrong");

            // Assert
            exception.Message.Should().Be("Something went wrong");
        }

        [Fact]
        public void Should_InheritFromException()
        {
            // Arrange & Act
            var exception = new TestDomainException("test");

            // Assert
            exception.Should().BeAssignableTo<Exception>();
        }
    }
}
