using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginCommandResponse>;
