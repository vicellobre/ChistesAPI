using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Events;

namespace Enjoy.Domain.UnitTests.Shared.Primitives.AggregateRoots;

public partial class AggregateRootTests
{
    public class GetDomainEventsMethod
    {
        [Fact]
        public void Should_ContainEvent_When_AfterCreate()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act
            var events = user.GetDomainEvents();

            // Assert
            events.Should().ContainSingle();
            events.Single().Should().BeOfType<UserRegisteredDomainEvent>();
        }

        [Fact]
        public void Should_ReturnNewCollection()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act
            var events1 = user.GetDomainEvents();
            var events2 = user.GetDomainEvents();

            // Assert
            events1.Should().NotBeSameAs(events2);
        }

        [Fact]
        public void Should_AccumulateEvents_When_MultipleOperations()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act
            user.ChangeName("Carlos");
            user.ChangeEmail("new@example.com");

            // Assert
            user.GetDomainEvents().Should().HaveCount(3);
        }
    }

    public class ClearDomainEventsMethod
    {
        [Fact]
        public void Should_RemoveAllEvents_When_Cleared()
        {
            // Arrange
            var user = User.Create("Vicente", "a@example.com", "$2a$10$hash", "User").Value;

            // Act
            user.ClearDomainEvents();

            // Assert
            user.GetDomainEvents().Should().BeEmpty();
        }
    }
}
