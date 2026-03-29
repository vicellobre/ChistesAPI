using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.Results;

public partial class ResultTests
{
    public class FailureMethod
    {
        [Fact]
        public void Should_ReturnFailureResult_When_Error()
        {
            // Arrange
            var error = Error.Validation("Test.Error", "Test message");

            // Act
            var result = Result.Failure(error);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
            result.FirstError.Should().Be(error);
        }

        [Fact]
        public void Should_ReturnFailureResult_When_ErrorCollection()
        {
            // Arrange
            ICollection<Error> errors = [
                Error.Validation("E1", "Error 1"),
                Error.Validation("E2", "Error 2")
            ];

            // Act
            var result = Result.Failure(errors);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ReturnFailureResult_When_Exception()
        {
            // Arrange
            var exception = new InvalidOperationException("Something failed");

            // Act
            var result = Result.Failure(exception);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Code.Should().Be("InvalidOperationException");
            result.FirstError.Message.Should().Be("Something failed");
        }

        [Fact]
        public void Should_ReturnTypedSuccessResult_When_GenericSuccess()
        {
            // Arrange & Act
            var result = Result.Success(42);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [Fact]
        public void Should_ReturnTypedFailure_When_GenericFailureWithError()
        {
            // Arrange
            var error = Error.Validation("Test", "msg");

            // Act
            var result = Result.Failure<int>(error);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTypedFailure_When_GenericFailureWithException()
        {
            // Arrange & Act
            var result = Result.Failure<int>(new Exception("fail"));

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTypedFailure_When_GenericFailureWithErrors()
        {
            // Arrange
            ICollection<Error> errors = [Error.Validation("E1", "err")];

            // Act
            var result = Result.Failure<int>(errors);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnSuccess_When_GenericCreateWithValue()
        {
            // Arrange & Act
            var result = Result.Create<string>("hello");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("hello");
        }

        [Fact]
        public void Should_ReturnFailure_When_GenericCreateWithNull()
        {
            // Arrange & Act
            var result = Result.Create<string>(null);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.NullValue);
        }
    }
}
