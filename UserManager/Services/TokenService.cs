using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserManager.Models;

namespace UserManager.Services;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public (string Token, DateTime ExpiresAt) CreateToken(User user, IEnumerable<string> roles)
    {
        var settings = configuration.GetSection("JwtSettings");
        var expiresAt = DateTime.UtcNow.AddMinutes(settings.GetValue<double>("ExpireMinutes"));
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email ?? string.Empty),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: settings["Issuer"],
            audience: settings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
