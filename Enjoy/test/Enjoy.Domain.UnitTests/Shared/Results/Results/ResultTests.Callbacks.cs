using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.Results;

public partial class ResultTests
{
    public class OnSuccessMethod
    {
        [Fact]
        public void Should_ExecuteAction_When_Success()
        {
            // Arrange
            var executed = false;
            var result = Result.Success();

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
            var result = Result.Failure(Error.Validation("E", "e"));

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
            var result = Result.Failure(Error.Validation("E", "e"));

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
            var result = Result.Success();

            // Act
            result.OnFailure(() => executed = true);

            // Assert
            executed.Should().BeFalse();
        }
    }
}
