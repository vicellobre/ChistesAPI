using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.UnitTests.Shared.Errors.Errors;

public partial class ErrorTests
{
    public class TopicErrors
    {
        [Fact]
        public void Should_ReturnErrorWithId_When_NotFound()
        {
            // Arrange & Act
            var error = Error.Topic.NotFound("topic_123");

            // Assert
            error.Code.Should().Be("Topic.NotFound");
            error.Type.Should().Be(ErrorType.NotFound);
            error.Message.Should().Contain("topic_123");
        }
    }
}
