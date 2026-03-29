using Enjoy.Domain.Topics.ValueObjects;

namespace Enjoy.Domain.UnitTests.Topics.ValueObjects.TopicNames;

public partial class TopicNameTests
{
    public class Constants
    {
        [Fact]
        public void MinLength_Should_Be2() =>
            TopicName.MinLength.Should().Be(2);

        [Fact]
        public void MaxLength_Should_Be100() =>
            TopicName.MaxLength.Should().Be(100);
    }
}
