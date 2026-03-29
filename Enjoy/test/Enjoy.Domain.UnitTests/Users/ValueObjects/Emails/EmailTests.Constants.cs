using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.UnitTests.Users.ValueObjects.Emails;

public partial class EmailTests
{
    public class Constants
    {
        [Fact]
        public void MinLength_Should_Be8() =>
            Email.MinLength.Should().Be(8);

        [Fact]
        public void MaxLength_Should_Be50() =>
            Email.MaxLength.Should().Be(50);

        [Fact]
        public void EmailPattern_Should_BeCorrect() =>
            Email.EmailPattern.Should().Be(@"^[^@]+@[^@]+$");
    }
}
