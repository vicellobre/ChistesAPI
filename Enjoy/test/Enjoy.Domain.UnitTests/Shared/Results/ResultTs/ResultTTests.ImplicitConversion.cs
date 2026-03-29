using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.ResultTs;

public partial class ResultTTests
{
    public class ImplicitConversion
    {
        [Fact]
        public void Should_ReturnSuccess_When_FromValue()
        {
            // Arrange & Act
            Result<string> result = "hello";

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("hello");
        }

        [Fact]
        public void Should_ReturnFailure_When_FromError()
        {
            // Arrange
            var error = Error.Validation("E", "e");

            // Act
            Result<string> result = error;

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(error);
        }

        [Fact]
        public void Should_ReturnFailure_When_FromErrorList()
        {
            // Arrange
            List<Error> errors = [Error.Validation("E1", "e1"), Error.Validation("E2", "e2")];

            // Act
            Result<string> result = errors;

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ReturnFailure_When_FromErrorHashSet()
        {
            // Arrange
            HashSet<Error> errors = [Error.Validation("E1", "e1")];

            // Act
            Result<string> result = errors;

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFailure_When_FromErrorArray()
        {
            // Arrange
            Error[] errors = [Error.Validation("E1", "e1")];

            // Act
            Result<string> result = errors;

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ConvertToSuccess_When_Success()
        {
            // Arrange
            var typed = Result<int>.Success(42);

            // Act
            Result result = typed;

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Should_ConvertToFailure_When_Failure()
        {
            // Arrange
            var typed = Result<int>.Failure(Error.Validation("E", "e"));

            // Act
            Result result = typed;

            // Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}
