using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.UnitTests.Shared.Extensions.CollectionExtensions;

public partial class CollectionExtensionsTests
{
    public class IsNullOrEmpty
    {
        [Fact]
        public void Should_ReturnTrue_When_EmptyCollection()
        {
            // Arrange
            List<int> collection = [];

            // Act
            var result = collection.IsNullOrEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NonEmptyCollection()
        {
            // Arrange
            List<int> collection = [1, 2];

            // Act
            var result = collection.IsNullOrEmpty();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_NullCollection()
        {
            // Arrange
            List<int> collection = null;

            // Act
            var result = collection!.IsNullOrEmpty();

            // Assert
            result.Should().BeTrue();
        }
    }

    public class IsNullOrEmptyReadOnlyMethod
    {
        [Fact]
        public void Should_ReturnTrue_When_NullCollection()
        {
            // Arrange
            IReadOnlyCollection<int>? collection = null;

            // Act
            var result = collection.IsNullOrEmptyReadOnly();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_EmptyReadOnlyCollection()
        {
            // Arrange
            IReadOnlyCollection<int> collection = [];

            // Act
            var result = collection.IsNullOrEmptyReadOnly();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NonEmptyReadOnlyCollection()
        {
            // Arrange
            IReadOnlyCollection<int> collection = [1, 2];

            // Act
            var result = collection.IsNullOrEmptyReadOnly();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsEmptyReadOnlyMethod
    {
        [Fact]
        public void Should_ReturnTrue_When_EmptyReadOnlyCollection()
        {
            // Arrange
            IReadOnlyCollection<int> collection = [];

            // Act
            var result = collection.IsEmptyReadOnly();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NonEmptyReadOnlyCollection()
        {
            // Arrange
            IReadOnlyCollection<int> collection = [1];

            // Act
            var result = collection.IsEmptyReadOnly();

            // Assert
            result.Should().BeFalse();
        }
    }
}
