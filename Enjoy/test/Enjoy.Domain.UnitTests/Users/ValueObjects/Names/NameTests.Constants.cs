using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Names;

public partial class NameTests
{
    public class Constants
    {
        [Fact]
        public void MinLength_Should_Be2() =>
            Name.MinLength.Should().Be(2);

        [Fact]
        public void MaxLength_Should_Be50() =>
            Name.MaxLength.Should().Be(50);

        [Fact]
        public void Pattern_Should_NotBeEmpty() =>
            Name.Pattern.Should().NotBeNullOrWhiteSpace();
    }
}
