using System.Reflection;
using Enjoy.Domain.Topics.Entities;

namespace Enjoy.Domain.UnitTests.Topics.Entities.Topics;

public partial class TopicTests
{
    public class ParameterlessConstructor
    {
        [Fact]
        public void Should_CreateInstance_When_UsingReflection()
        {
            // Arrange
            var ctor = typeof(Topic).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null, types: Type.EmptyTypes, modifiers: null);

            // Act
            var topic = (Topic)ctor!.Invoke(null);

            // Assert
            topic.Should().NotBeNull();
        }
    }
}
