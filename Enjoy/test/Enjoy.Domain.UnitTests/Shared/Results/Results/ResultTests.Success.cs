using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.Results;

public partial class ResultTests
{
    public class SuccessMethod
    {
        [Fact]
        public void Should_ReturnSuccessResult()
        {
            // Arrange & Act
            var result = Result.Success();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnNone()
        {
            // Arrange & Act
            var result = Result.Success();

            // Assert
            result.FirstError.Should().Be(Error.None);
        }
    }
}
