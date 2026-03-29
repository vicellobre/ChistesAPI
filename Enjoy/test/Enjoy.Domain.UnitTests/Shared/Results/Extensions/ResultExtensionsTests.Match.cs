using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.Extensions;

public partial class ResultExtensionsTests
{
    public class Match
    {
        [Fact]
        public void Should_InvokeOnSuccess_When_TypedSuccess()
        {
            // Arrange
            var result = Result<int>.Success(42);

            // Act
            var output = result.Match(
                value => $"Value: {value}",
                error => $"Error: {error.Code}");

            // Assert
            output.Should().Be("Value: 42");
        }

        [Fact]
        public void Should_InvokeOnFailure_When_TypedFailure()
        {
            // Arrange
            var result = Result<int>.Failure(Error.Validation("E", "msg"));

            // Act
            var output = result.Match(
                value => $"Value: {value}",
                error => $"Error: {error.Code}");

            // Assert
            output.Should().Be("Error: E");
        }

        [Fact]
        public void Should_InvokeOnSuccess_When_UntypedSuccess()
        {
            // Arrange
            var result = Result.Success();

            // Act
            var output = result.Match(
                () => "ok",
                error => $"Error: {error.Code}");

            // Assert
            output.Should().Be("ok");
        }

        [Fact]
        public void Should_InvokeOnFailure_When_UntypedFailure()
        {
            // Arrange
            var result = Result.Failure(Error.Validation("E", "msg"));

            // Act
            var output = result.Match(
                () => "ok",
                error => $"Error: {error.Code}");

            // Assert
            output.Should().Be("Error: E");
        }
    }
}
