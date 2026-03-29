using System.Reflection;
using Enjoy.Domain.Shared.Primitives;

namespace Enjoy.Domain.UnitTests.Shared.Primitives.Entities;

public partial class EntityTests
{
    private sealed class TestEntity : Entity
    {
        public TestEntity(string id) : base(id) { }
        public TestEntity() : base() { }
    }

    public class Constructor
    {
        [Fact]
        public void Should_SetId_When_IdProvided()
        {
            // Arrange & Act
            var entity = new TestEntity("test-id");

            // Assert
            entity.Id.Should().Be("test-id");
        }

        [Fact]
        public void Should_CreateInstance_When_Parameterless()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.Should().NotBeNull();
        }
    }
}
