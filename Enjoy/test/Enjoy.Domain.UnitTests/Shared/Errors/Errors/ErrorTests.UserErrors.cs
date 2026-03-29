using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Shared.Errors.Errors;

public partial class ErrorTests
{
    public class UserErrors
    {
        [Fact]
        public void Should_HaveValidationType()
        {
            // Arrange & Act & Assert
            Error.User.IsNull.Code.Should().Be("User.IsNull");
            Error.User.IsNull.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Should_HaveConflictType()
        {
            // Arrange & Act & Assert
            Error.User.EmailAlreadyInUse.Code.Should().Be("User.EmailAlreadyInUse");
            Error.User.EmailAlreadyInUse.Type.Should().Be(ErrorType.Conflict);
        }

        [Fact]
        public void Should_ReturnErrorWithId_When_NotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var error = Error.User.NotFound(id);

            // Assert
            error.Code.Should().Be("User.NotFound");
            error.Type.Should().Be(ErrorType.NotFound);
            error.Message.Should().Contain(id.ToString());
        }

        [Fact]
        public void Should_ReturnErrorWithEmail_When_EmailNotExist()
        {
            // Arrange & Act
            var error = Error.User.EmailNotExist("test@example.com");

            // Assert
            error.Code.Should().Be("User.EmailNotExist");
            error.Type.Should().Be(ErrorType.NotFound);
            error.Message.Should().Contain("test@example.com");
        }

        [Fact]
        public void Should_HaveUnauthorizedType()
        {
            // Arrange & Act & Assert
            Error.User.InvalidCredentials.Code.Should().Be("User.InvalidCredentials");
            Error.User.InvalidCredentials.Type.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public void Should_HaveNotFoundType()
        {
            // Arrange & Act & Assert
            Error.User.NoUsersFound.Code.Should().Be("User.NoUsersFound");
            Error.User.NoUsersFound.Type.Should().Be(ErrorType.NotFound);
        }
    }
}
