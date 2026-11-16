using UserManager.Models;

namespace UserManager.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user, IEnumerable<string> roles);
}
