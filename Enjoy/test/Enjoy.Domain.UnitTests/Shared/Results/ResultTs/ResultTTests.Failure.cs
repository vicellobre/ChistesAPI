using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.ResultTs;

public partial class ResultTTests
{
    public class FailureMethod
    {
        [Fact]
        public void Should_ReturnFailure_When_Error()
        {
            // Arrange
            var error = Error.Validation("Test", "msg");

            // Act
            var result = Result<int>.Failure(error);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(error);
        }

        [Fact]
        public void Should_FallBackToNullValue_When_ErrorNone()
        {
            // Arrange & Act
            var result = Result<int>.Failure(Error.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.NullValue);
        }

        [Fact]
        public void Should_ReturnFailure_When_Exception()
        {
            // Arrange & Act
            var result = Result<int>.Failure(new InvalidOperationException("fail"));

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Code.Should().Be("InvalidOperationException");
        }

        [Fact]
        public void Should_ReturnFailure_When_ErrorCollection()
        {
            // Arrange
            ICollection<Error> errors = [
                Error.Validation("E1", "err1"),
                Error.Validation("E2", "err2")
            ];

            // Act
            var result = Result<int>.Failure(errors);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void Should_FallBackToNullValue_When_EmptyErrorCollection()
        {
            // Arrange
            ICollection<Error> errors = [];

            // Act
            var result = Result<int>.Failure(errors);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.NullValue);
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_AccessingValue()
        {
            // Arrange
            var result = Result<int>.Failure(Error.Validation("E", "e"));

            // Act
            var act = () => result.Value;

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
