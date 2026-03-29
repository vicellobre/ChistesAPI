using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Names;

public partial class NameTests
{
    public class Create
    {
        [Fact]
        public void Should_ReturnSuccess_When_NameIsValid()
        {
            // Arrange
            const string name = "Vicente";

            // Act
            var result = Name.Create(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Vicente");
        }

        [Fact]
        public void Should_CapitalizeWords_When_NameIsValid()
        {
            // Arrange
            const string name = "juan carlos";

            // Act
            var result = Name.Create(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Juan Carlos");
        }

        [Fact]
        public void Should_TrimWhitespace_When_NameIsValid()
        {
            // Arrange
            const string name = "  Vicente  ";

            // Act
            var result = Name.Create(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Vicente");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnIsNullOrEmptyError_When_NullOrWhitespace(string? value)
        {
            // Arrange & Act
            var result = Name.Create(value!);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.Name.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnTooShortError_When_TooShort()
        {
            // Arrange
            const string name = "A";

            // Act
            var result = Name.Create(name);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Name.TooShort(Name.MinLength));
        }

        [Fact]
        public void Should_ReturnTooLongError_When_TooLong()
        {
            // Arrange
            var name = new string('A', Name.MaxLength + 1);

            // Act
            var result = Name.Create(name);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Name.TooLong(Name.MaxLength));
        }

        [Theory]
        [InlineData("Juan123")]
        [InlineData("Name!@#")]
        [InlineData("Test_Name")]
        public void Should_ReturnInvalidFormatError_When_InvalidFormat(string name)
        {
            // Arrange & Act
            var result = Name.Create(name);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Name.InvalidFormat);
        }

        [Theory]
        [InlineData("María")]
        [InlineData("José")]
        [InlineData("François")]
        [InlineData("Müller")]
        public void Should_ReturnSuccess_When_UnicodeLetters(string name)
        {
            // Arrange & Act
            var result = Name.Create(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnMultipleErrors_When_TooShortAndInvalidFormat()
        {
            // Arrange
            const string name = "1";

            // Act
            var result = Name.Create(name);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        }
    }
}
