namespace Enjoy.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    AccessTokenResponse Create(string userId, string email, IEnumerable<string> roles);
}
