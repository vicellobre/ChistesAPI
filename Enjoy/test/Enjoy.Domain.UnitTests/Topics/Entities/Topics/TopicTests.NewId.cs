using Enjoy.Domain.Topics.Entities;

namespace Enjoy.Domain.UnitTests.Topics.Entities.Topics;

public partial class TopicTests
{
    public class NewId
    {
        [Fact]
        public void Should_StartWithPrefix()
        {
            // Arrange & Act
            var id = Topic.NewId();

            // Assert
            id.Should().StartWith("topic_");
        }

        [Fact]
        public void Should_GenerateUniqueIds()
        {
            // Arrange & Act
            var id1 = Topic.NewId();
            var id2 = Topic.NewId();

            // Assert
            id1.Should().NotBe(id2);
        }
    }
}
