using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.ResultTs;

public partial class ResultTTests
{
    public class SuccessMethod
    {
        [Fact]
        public void Should_ReturnSuccessResult_When_Value()
        {
            // Arrange & Act
            var result = Result<int>.Success(42);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Value.Should().Be(42);
        }

        [Fact]
        public void Should_ReturnNone()
        {
            // Arrange & Act
            var result = Result<int>.Success(42);

            // Assert
            result.FirstError.Should().Be(Error.None);
        }
    }
}
