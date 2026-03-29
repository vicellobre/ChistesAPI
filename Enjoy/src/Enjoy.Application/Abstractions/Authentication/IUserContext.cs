namespace Enjoy.Application.Abstractions.Authentication;

public interface IUserContext
{
    string? GetUserId();
    string? GetUserRole();
}
