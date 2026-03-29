using Enjoy.Application.Jokes.Commands.CreateJoke;

namespace Enjoy.Presentation.Jokes.V1.Models.CreateJoke;

public static class CreateJokeExtensions
{
    public static CreateJokeCommand ToCommand(this CreateJokeRequest request) =>
        new(request.Text);

    public static CreateJokeResponse ToResponse(this CreateJokeCommandResponse response) =>
        new(response.Id);
}
