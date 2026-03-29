using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Jokes.Commands.CreateJoke;

public sealed record CreateJokeCommand(string Text) : ICommand<CreateJokeCommandResponse>;
