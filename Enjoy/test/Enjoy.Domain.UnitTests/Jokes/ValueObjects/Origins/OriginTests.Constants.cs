using Enjoy.Domain.Jokes.ValueObjects;

namespace Enjoy.Domain.UnitTests.Jokes.ValueObjects.Origins;

public partial class OriginTests
{
    public class Constants
    {
        [Fact]
        public void ChuckNorris_Should_BeChuckNorris() =>
            Origin.ChuckNorris.Should().Be("ChuckNorris");

        [Fact]
        public void DadJoke_Should_BeDadJoke() =>
            Origin.DadJoke.Should().Be("DadJoke");

        [Fact]
        public void Local_Should_BeLocal() =>
            Origin.Local.Should().Be("Local");

        [Fact]
        public void All_Should_ContainAllValidValues()
        {
            // Arrange & Act & Assert
            Origin.All.Should().HaveCount(3);
            Origin.All.Should().Contain("ChuckNorris");
            Origin.All.Should().Contain("DadJoke");
            Origin.All.Should().Contain("Local");
        }
    }
}
