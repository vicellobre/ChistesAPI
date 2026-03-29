using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Shared.Errors.Errors;

public partial class ErrorTests
{
    public class Common
    {
        [Fact]
        public void Should_HaveEmptyCodeAndMessage_When_None()
        {
            // Arrange & Act & Assert
            Error.None.Code.Should().Be(string.Empty);
            Error.None.Message.Should().Be(string.Empty);
            Error.None.Type.Should().Be(ErrorType.None);
        }

        [Fact]
        public void Should_HaveValidationType_When_NullValue()
        {
            // Arrange & Act & Assert
            Error.NullValue.Type.Should().Be(ErrorType.Validation);
            Error.NullValue.Code.Should().Be("Error.NullValue");
        }

        [Fact]
        public void Should_HaveUnexpectedType_When_Unknown()
        {
            // Arrange & Act & Assert
            Error.Unknown.Type.Should().Be(ErrorType.Unexpected);
            Error.Unknown.Code.Should().Be("Error.Unknown");
        }

        [Fact]
        public void Should_BeEmpty_When_EmptyErrors()
        {
            // Arrange & Act & Assert
            Error.EmptyErrors.Should().BeEmpty();
        }
    }
}
