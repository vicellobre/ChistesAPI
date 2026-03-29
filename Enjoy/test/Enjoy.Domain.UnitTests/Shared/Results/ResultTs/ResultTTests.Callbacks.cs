using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.ResultTs;

public partial class ResultTTests
{
    public class OnSuccessMethod
    {
        [Fact]
        public void Should_ExecuteAction_When_Success()
        {
            // Arrange
            var executed = false;
            var result = Result<int>.Success(1);

            // Act
            result.OnSuccess(() => executed = true);

            // Assert
            executed.Should().BeTrue();
        }

        [Fact]
        public void Should_NotExecuteAction_When_Failure()
        {
            // Arrange
            var executed = false;
            var result = Result<int>.Failure(Error.Validation("E", "e"));

            // Act
            result.OnSuccess(() => executed = true);

            // Assert
            executed.Should().BeFalse();
        }
    }

    public class OnFailureMethod
    {
        [Fact]
        public void Should_ExecuteAction_When_Failure()
        {
            // Arrange
            var executed = false;
            var result = Result<int>.Failure(Error.Validation("E", "e"));

            // Act
            result.OnFailure(() => executed = true);

            // Assert
            executed.Should().BeTrue();
        }

        [Fact]
        public void Should_NotExecuteAction_When_Success()
        {
            // Arrange
            var executed = false;
            var result = Result<int>.Success(1);

            // Act
            result.OnFailure(() => executed = true);

            // Assert
            executed.Should().BeFalse();
        }
    }

    public class GetValueOrDefaultMethod
    {
        [Fact]
        public void Should_ReturnValue_When_Success()
        {
            // Arrange
            var result = Result<int>.Success(42);

            // Act
            var value = result.GetValueOrDefault(0);

            // Assert
            value.Should().Be(42);
        }

        [Fact]
        public void Should_ReturnDefault_When_Failure()
        {
            // Arrange
            var result = Result<int>.Failure(Error.Validation("E", "e"));

            // Act
            var value = result.GetValueOrDefault(99);

            // Assert
            value.Should().Be(99);
        }
    }
}
