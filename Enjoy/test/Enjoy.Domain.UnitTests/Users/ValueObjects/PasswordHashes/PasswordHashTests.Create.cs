using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.PasswordHashes;

public partial class PasswordHashTests
{
    public class Create
    {
        [Fact]
        public void Should_ReturnSuccess_When_HashIsValid()
        {
            // Arrange
            const string hash = "$2a$10$somevalidhash";

            // Act
            var result = PasswordHash.Create(hash);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(hash);
        }

        [Fact]
        public void Should_TrimWhitespace_When_HashIsValid()
        {
            // Arrange
            const string hash = "  $2a$10$somevalidhash  ";

            // Act
            var result = PasswordHash.Create(hash);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("$2a$10$somevalidhash");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnIsNullOrEmptyError_When_NullOrWhitespace(string? value)
        {
            // Arrange & Act
            var result = PasswordHash.Create(value);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.PasswordHash.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnTooLongError_When_TooLong()
        {
            // Arrange
            var hash = new string('x', PasswordHash.MaxLength + 1);

            // Act
            var result = PasswordHash.Create(hash);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.PasswordHash.TooLong(PasswordHash.MaxLength));
        }

        [Fact]
        public void Should_ReturnTooShortError_When_TooShort()
        {
            // Arrange
            var hash = new string('x', PasswordHash.MinLength - 1);

            // Act
            var result = PasswordHash.Create(hash);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.PasswordHash.TooShort(PasswordHash.MinLength));
        }

        [Fact]
        public void Should_ReturnSuccess_When_ExactMinLength()
        {
            // Arrange
            var hash = new string('x', PasswordHash.MinLength);

            // Act
            var result = PasswordHash.Create(hash);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnSuccess_When_ExactMaxLength()
        {
            // Arrange
            var hash = new string('x', PasswordHash.MaxLength);

            // Act
            var result = PasswordHash.Create(hash);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
