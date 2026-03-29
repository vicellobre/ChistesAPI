using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Auth.Commands.ExternalLoginCallback;

public sealed record ExternalLoginCallbackCommand(
    string Code,
    string State,
    string Provider) : ICommand<ExternalLoginCallbackCommandResponse>;
