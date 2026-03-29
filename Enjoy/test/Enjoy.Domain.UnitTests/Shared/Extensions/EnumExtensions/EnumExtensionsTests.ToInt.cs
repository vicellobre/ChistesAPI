using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.EnumExtensions;

public partial class EnumExtensionsTests
{
    public class ToInt
    {
        [Theory]
        [InlineData(ErrorType.None, 0)]
        [InlineData(ErrorType.Failure, 1)]
        [InlineData(ErrorType.Unexpected, 2)]
        [InlineData(ErrorType.Validation, 3)]
        [InlineData(ErrorType.Conflict, 4)]
        [InlineData(ErrorType.NotFound, 5)]
        [InlineData(ErrorType.Unauthorized, 6)]
        [InlineData(ErrorType.Forbidden, 7)]
        [InlineData(ErrorType.Exception, 8)]
        [InlineData(ErrorType.TooManyRequests, 9)]
        public void Should_ReturnExpectedInt_When_ErrorType(ErrorType type, int expected)
        {
            // Arrange & Act
            var result = type.ToInt();

            // Assert
            result.Should().Be(expected);
        }
    }
}
