using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.CollectionExtensions;

public partial class CollectionExtensionsTests
{
    public class IsEmpty
    {
        [Fact]
        public void Should_ReturnTrue_When_EmptyCollection()
        {
            // Arrange
            List<int> collection = [];

            // Act
            var result = collection.IsEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NonEmptyCollection()
        {
            // Arrange
            List<int> collection = [1, 2, 3];

            // Act
            var result = collection.IsEmpty();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsNull
    {
        [Fact]
        public void Should_ReturnFalse_When_NotNull()
        {
            // Arrange
            ICollection<int> collection = [1, 2, 3];

            // Act
            var result = collection.IsNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_Null()
        {
            // Arrange
            ICollection<int> collection = null;

            // Act
            var result = collection!.IsNull();

            // Assert
            result.Should().BeTrue();
        }
    }
}
