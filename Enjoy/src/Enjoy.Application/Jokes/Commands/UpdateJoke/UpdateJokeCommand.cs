using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Jokes.Commands.UpdateJoke;

public sealed record UpdateJokeCommand(
    string Id,
    string NewText,
    string RequesterId,
    string RequesterRole) : ICommand;
