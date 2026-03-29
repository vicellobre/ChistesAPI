using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.PasswordHashes;

public partial class PasswordHashTests
{
    public class Constants
    {
        [Fact]
        public void MinLength_Should_Be8() =>
            PasswordHash.MinLength.Should().Be(8);

        [Fact]
        public void MaxLength_Should_Be500() =>
            PasswordHash.MaxLength.Should().Be(500);
    }
}
