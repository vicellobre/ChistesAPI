using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Shared.Errors.Errors;

public partial class ErrorTests
{
    public class WithStackMethod
    {
        [Fact]
        public void Should_ReturnErrorWithStack_When_FromError()
        {
            // Arrange
            var original = Error.Validation("V.Code", "validation error");
            IReadOnlyCollection<Error> stack = [original];

            // Act
            var error = Error.WithStack(original, stack);

            // Assert
            error.Code.Should().Be("V.Code");
            error.StackTrace.Should().HaveCount(1);
        }

        [Fact]
        public void Should_ReturnErrorFromFirstInStack_When_FromCollection()
        {
            // Arrange
            var first = Error.Validation("First", "first error");
            var second = Error.Validation("Second", "second error");
            IReadOnlyCollection<Error> stack = [first, second];

            // Act
            var error = Error.WithStack(stack);

            // Assert
            error.Code.Should().Be("First");
            error.Message.Should().Be("first error");
            error.StackTrace.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_NullCollection()
        {
            // Arrange & Act
            var act = () => Error.WithStack(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_EmptyCollection()
        {
            // Arrange
            IReadOnlyCollection<Error> empty = [];

            // Act
            var act = () => Error.WithStack(empty);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_AttachStack_When_Create()
        {
            // Arrange
            var inner = Error.Validation("Inner", "inner");
            IReadOnlyCollection<Error> stack = [inner];

            // Act
            var error = Error.Failure("Outer", "outer", stack);

            // Assert
            error.StackTrace.Should().HaveCount(1);
        }
    }
}
