using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.Extensions;

public partial class ResultExtensionsTests
{
    public class ToResult
    {
        [Fact]
        public void Should_ReturnSuccess_When_FromValue()
        {
            // Arrange & Act
            var result = "hello".ToResult();

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
            var result = error.ToResult<string>();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFailure_When_FromErrorCollection()
        {
            // Arrange
            ICollection<Error> errors = [Error.Validation("E1", "e1")];

            // Act
            var result = errors.ToResult<string>();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFailure_When_FromErrorList()
        {
            // Arrange
            List<Error> errors = [Error.Validation("E1", "e1")];

            // Act
            var result = errors.ToResult<string>();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFailure_When_FromErrorHashSet()
        {
            // Arrange
            HashSet<Error> errors = [Error.Validation("E1", "e1")];

            // Act
            var result = errors.ToResult<string>();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFailure_When_FromErrorArray()
        {
            // Arrange
            Error[] errors = [Error.Validation("E1", "e1")];

            // Act
            var result = errors.ToResult<string>();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}
