using Enjoy.Domain.Jokes.ValueObjects;

namespace Enjoy.Domain.UnitTests.Jokes.ValueObjects.JokeTexts;

public partial class JokeTextTests
{
    public class Constants
    {
        [Fact]
        public void MaxLength_Should_Be2000() =>
            JokeText.MaxLength.Should().Be(2000);
    }
}
