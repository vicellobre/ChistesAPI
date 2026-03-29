using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword) : ICommand<RegisterCommandResponse>;
