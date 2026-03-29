using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Emails;

public partial class EmailTests
{
    public class Create
    {
        [Fact]
        public void Should_ReturnSuccess_When_EmailIsValid()
        {
            // Arrange
            const string email = "user@example.com";

            // Act
            var result = Email.Create(email);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(email);
        }

        [Fact]
        public void Should_NormalizeToLowercase_When_EmailIsValid()
        {
            // Arrange
            const string email = "User@Example.COM";

            // Act
            var result = Email.Create(email);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("user@example.com");
        }

        [Fact]
        public void Should_TrimWhitespace_When_EmailIsValid()
        {
            // Arrange
            const string email = "  user@example.com  ";

            // Act
            var result = Email.Create(email);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("user@example.com");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ReturnIsNullOrEmptyError_When_NullOrWhitespace(string? value)
        {
            // Arrange & Act
            var result = Email.Create(value!);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.Email.IsNullOrEmpty);
        }

        [Fact]
        public void Should_ReturnTooShortError_When_TooShort()
        {
            // Arrange
            var email = "a@b.com";

            // Act
            var result = Email.Create(email);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Email.TooShort(Email.MinLength));
        }

        [Fact]
        public void Should_ReturnTooLongError_When_TooLong()
        {
            // Arrange
            var email = new string('a', Email.MaxLength) + "@example.com";

            // Act
            var result = Email.Create(email);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Email.TooLong(Email.MaxLength));
        }

        [Theory]
        [InlineData("noatsign")]
        [InlineData("missing-domain@")]
        [InlineData("@missing-local")]
        public void Should_ReturnInvalidFormatError_When_InvalidFormat(string email)
        {
            // Arrange & Act
            var result = Email.Create(email);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(Error.Email.InvalidFormat);
        }

        [Fact]
        public void Should_ReturnMultipleErrors_When_TooShortAndInvalidFormat()
        {
            // Arrange
            const string email = "abc";

            // Act
            var result = Email.Create(email);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
            result.Errors.Should().Contain(Error.Email.TooShort(Email.MinLength));
            result.Errors.Should().Contain(Error.Email.InvalidFormat);
        }
    }
}
