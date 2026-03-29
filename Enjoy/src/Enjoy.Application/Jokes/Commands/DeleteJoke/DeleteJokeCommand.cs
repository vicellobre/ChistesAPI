using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Jokes.Commands.DeleteJoke;

public sealed record DeleteJokeCommand(
    string Id,
    string RequesterId,
    string RequesterRole) : ICommand;
