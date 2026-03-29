using System.Reflection;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.UnitTests.Shared.Results.Results;

public partial class ResultTests
{
    public class Constructor
    {
        [Fact]
        public void Should_ThrowInvalidOperationException_When_ParameterlessConstructor()
        {
            // Arrange & Act
            var act = () => new Result();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }

    public class ConstructorWithErrorCollection
    {
        private static ConstructorInfo GetCtor() =>
            typeof(Result).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: [typeof(bool), typeof(ICollection<Error>)],
                modifiers: null)!;

        [Fact]
        public void Should_BeSuccess_When_TrueAndEmptyErrors()
        {
            // Arrange
            var ctor = GetCtor();
            ICollection<Error> empty = Error.EmptyErrors;

            // Act
            var result = (Result)ctor.Invoke([true, empty]);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Should_BeFailure_When_FalseAndNonEmptyErrors()
        {
            // Arrange
            var ctor = GetCtor();
            ICollection<Error> errors = [Error.Validation("E", "e")];

            // Act
            var result = (Result)ctor.Invoke([false, errors]);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Code.Should().Be("E");
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_SuccessWithErrors()
        {
            // Arrange
            var ctor = GetCtor();
            ICollection<Error> errors = [Error.Validation("E", "e")];

            // Act
            var act = () => ctor.Invoke([true, errors]);

            // Assert
            act.Should().Throw<TargetInvocationException>()
                .WithInnerException<InvalidOperationException>();
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_FailureWithEmptyErrors()
        {
            // Arrange
            var ctor = GetCtor();
            ICollection<Error> empty = [];

            // Act
            var act = () => ctor.Invoke([false, empty]);

            // Assert
            act.Should().Throw<TargetInvocationException>()
                .WithInnerException<InvalidOperationException>();
        }
    }

    public class ConstructorWithError
    {
        private static ConstructorInfo GetCtor() =>
            typeof(Result).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: [typeof(bool), typeof(Error)],
                modifiers: null)!;

        [Fact]
        public void Should_BeSuccess_When_TrueAndNoneError()
        {
            // Arrange
            var ctor = GetCtor();

            // Act
            var result = (Result)ctor.Invoke([true, Error.None]);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Should_BeFailure_When_FalseAndNonNoneError()
        {
            // Arrange
            var ctor = GetCtor();
            var error = Error.Validation("E", "e");

            // Act
            var result = (Result)ctor.Invoke([false, error]);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.FirstError.Should().Be(error);
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_SuccessWithNonNoneError()
        {
            // Arrange
            var ctor = GetCtor();
            var error = Error.Validation("E", "e");

            // Act
            var act = () => ctor.Invoke([true, error]);

            // Assert
            act.Should().Throw<TargetInvocationException>()
                .WithInnerException<InvalidOperationException>();
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_FailureWithNoneError()
        {
            // Arrange
            var ctor = GetCtor();

            // Act
            var act = () => ctor.Invoke([false, Error.None]);

            // Assert
            act.Should().Throw<TargetInvocationException>()
                .WithInnerException<InvalidOperationException>();
        }
    }
}
