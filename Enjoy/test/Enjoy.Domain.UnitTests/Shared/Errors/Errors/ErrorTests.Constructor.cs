using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Shared.Errors.Errors;

public partial class ErrorTests
{
    public class Constructor
    {
        [Fact]
        public void Should_ThrowInvalidOperationException_When_ParameterlessConstructor()
        {
            // Arrange & Act
            var act = () => new Error();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_NullCode()
        {
            // Arrange & Act
            var act = () => Error.Create(null!, "message", ErrorType.Failure);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_NullMessage()
        {
            // Arrange & Act
            var act = () => Error.Create("code", null!, ErrorType.Failure);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_SetEmptyString_When_WhitespaceCode()
        {
            // Arrange & Act
            var error = Error.Create("   ", "message", ErrorType.Failure);

            // Assert
            error.Code.Should().Be(string.Empty);
        }

        [Fact]
        public void Should_SetEmptyString_When_WhitespaceMessage()
        {
            // Arrange & Act
            var error = Error.Create("code", "   ", ErrorType.Failure);

            // Assert
            error.Message.Should().Be(string.Empty);
        }
    }
}
