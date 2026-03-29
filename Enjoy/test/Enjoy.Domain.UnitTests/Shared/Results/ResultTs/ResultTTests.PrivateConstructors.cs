using System.Reflection;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.ResultTs;

public partial class ResultTTests
{
    public class Constructor
    {
        [Fact]
        public void Should_ThrowInvalidOperationException_When_ParameterlessConstructor()
        {
            // Arrange & Act
            var act = () => new Result<int>();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }

    public class ConstructorWithValue
    {
        private static ConstructorInfo GetCtor() =>
            typeof(Result<string>).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: [typeof(string)],
                modifiers: null)!;

        [Fact]
        public void Should_BeSuccess_When_ValueIsNotNull()
        {
            // Arrange
            var ctor = GetCtor();

            // Act
            var result = (Result<string>)ctor.Invoke(["hello"]);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("hello");
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Should_HaveNullValueError_When_ValueIsNull()
        {
            // Arrange
            var ctor = GetCtor();

            // Act
            var result = (Result<string>)ctor.Invoke([null]);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.NullValue);
        }
    }

    public class ConstructorWithError
    {
        private static ConstructorInfo GetCtor() =>
            typeof(Result<string>).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: [typeof(Error)],
                modifiers: null)!;

        [Fact]
        public void Should_BeFailure_When_ErrorIsNotNone()
        {
            // Arrange
            var ctor = GetCtor();
            var error = Error.Validation("E", "e");

            // Act
            var result = (Result<string>)ctor.Invoke([error]);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(error);
        }

        [Fact]
        public void Should_FallBackToNullValue_When_ErrorIsNone()
        {
            // Arrange
            var ctor = GetCtor();

            // Act
            var result = (Result<string>)ctor.Invoke([Error.None]);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.NullValue);
        }
    }

    public class ConstructorWithErrors
    {
        private static ConstructorInfo GetCtor() =>
            typeof(Result<string>).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: [typeof(ICollection<Error>)],
                modifiers: null)!;

        [Fact]
        public void Should_BeFailure_When_ErrorsIsNotEmpty()
        {
            // Arrange
            var ctor = GetCtor();
            ICollection<Error> errors = [Error.Validation("E1", "e1"), Error.Validation("E2", "e2")];

            // Act
            var result = (Result<string>)ctor.Invoke([errors]);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void Should_FallBackToNullValue_When_ErrorsIsEmpty()
        {
            // Arrange
            var ctor = GetCtor();
            ICollection<Error> empty = [];

            // Act
            var result = (Result<string>)ctor.Invoke([empty]);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.NullValue);
        }

        [Fact]
        public void Should_FallBackToNullValue_When_ErrorsIsNull()
        {
            // Arrange
            var ctor = GetCtor();

            // Act
            var result = (Result<string>)ctor.Invoke([null]);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(Error.NullValue);
        }
    }
}
