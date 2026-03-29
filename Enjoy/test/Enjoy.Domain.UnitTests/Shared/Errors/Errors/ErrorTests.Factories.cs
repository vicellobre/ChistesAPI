using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Shared.Errors.Errors;

public partial class ErrorTests
{
    public class Factories
    {
        [Fact]
        public void Should_ReturnErrorWithCorrectProperties_When_Create()
        {
            // Arrange & Act
            var error = Error.Create("Code", "Message", ErrorType.Failure);

            // Assert
            error.Code.Should().Be("Code");
            error.Message.Should().Be("Message");
            error.Type.Should().Be(ErrorType.Failure);
        }

        [Fact]
        public void Should_ReturnFailureType()
        {
            // Arrange & Act
            var error = Error.Failure("F.Code", "fail");

            // Assert
            error.Type.Should().Be(ErrorType.Failure);
        }

        [Fact]
        public void Should_ReturnUnexpectedType()
        {
            // Arrange & Act
            var error = Error.Unexpected("U.Code", "unexpected");

            // Assert
            error.Type.Should().Be(ErrorType.Unexpected);
        }

        [Fact]
        public void Should_ReturnValidationType()
        {
            // Arrange & Act
            var error = Error.Validation("V.Code", "invalid");

            // Assert
            error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Should_ReturnConflictType()
        {
            // Arrange & Act
            var error = Error.Conflict("C.Code", "conflict");

            // Assert
            error.Type.Should().Be(ErrorType.Conflict);
        }

        [Fact]
        public void Should_ReturnNotFoundType()
        {
            // Arrange & Act
            var error = Error.NotFound("NF.Code", "not found");

            // Assert
            error.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void Should_ReturnUnauthorizedType()
        {
            // Arrange & Act
            var error = Error.Unauthorized("UA.Code", "unauthorized");

            // Assert
            error.Type.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public void Should_ReturnForbiddenType()
        {
            // Arrange & Act
            var error = Error.Forbidden("FB.Code", "forbidden");

            // Assert
            error.Type.Should().Be(ErrorType.Forbidden);
        }

        [Fact]
        public void Should_ReturnExceptionType()
        {
            // Arrange & Act
            var error = Error.Exception("EX.Code", "exception");

            // Assert
            error.Type.Should().Be(ErrorType.Exception);
        }

        [Fact]
        public void Should_ReturnTooManyRequestsType()
        {
            // Arrange & Act
            var error = Error.TooManyRequests("TM.Code", "rate limit");

            // Assert
            error.Type.Should().Be(ErrorType.TooManyRequests);
        }
    }
}
