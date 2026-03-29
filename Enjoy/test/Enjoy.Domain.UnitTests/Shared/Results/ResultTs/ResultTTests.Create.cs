using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.ResultTs;

public partial class ResultTTests
{
    public class CreateMethod
    {
        [Fact]
        public void Should_ReturnSuccess_When_Value()
        {
            // Arrange & Act
            var result = Result<string>.Create("hello");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("hello");
        }

        [Fact]
        public void Should_ReturnFailureWithNullValue_When_Null()
        {
            // Arrange & Act
            var result = Result<string>.Create(null);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.NullValue);
        }
    }
}
