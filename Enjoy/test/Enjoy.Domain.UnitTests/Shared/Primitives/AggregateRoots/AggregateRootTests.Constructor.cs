using Enjoy.Domain.Shared.Primitives;

namespace Enjoy.Domain.UnitTests.Shared.Primitives.AggregateRoots;

public partial class AggregateRootTests
{
    private sealed class TestAggregateRoot : AggregateRoot
    {
        public TestAggregateRoot(string id) : base(id) { }
        public TestAggregateRoot() : base() { }
    }

    public class Constructor
    {
        [Fact]
        public void Should_SetId_When_IdProvided()
        {
            // Arrange & Act
            var root = new TestAggregateRoot("agg-id");

            // Assert
            root.Id.Should().Be("agg-id");
        }

        [Fact]
        public void Should_CreateInstance_When_Parameterless()
        {
            // Arrange & Act
            var root = new TestAggregateRoot();

            // Assert
            root.Should().NotBeNull();
            root.GetDomainEvents().Should().BeEmpty();
        }
    }
}
