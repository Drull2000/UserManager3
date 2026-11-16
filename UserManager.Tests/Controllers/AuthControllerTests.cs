using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using UserManager.Controllers;
using UserManager.Models;
using UserManager.Services;

namespace UserManager.Tests.Controllers;

public sealed class AuthControllerTests
{
    private const string SecretKey = "test-secret-key-with-at-least-32-characters";

    [Fact]
    public void TokenService_CreatesJwtWithUserAndRoleClaims()
    {
        var configuration = CreateConfiguration();
        var tokenService = new TokenService(configuration);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            FirstName = "Test",
            LastName = "User"
        };

        var (token, expiresAt) = tokenService.CreateToken(user, ["User", "Admin"]);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(user.Id.ToString(), jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(user.Email, jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Contains(jwt.Claims, claim => claim.Type == System.Security.Claims.ClaimTypes.Role && claim.Value == "User");
        Assert.Contains(jwt.Claims, claim => claim.Type == System.Security.Claims.ClaimTypes.Role && claim.Value == "Admin");
        Assert.True(expiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void AuthController_AuthenticationEndpointsAllowAnonymous()
    {
        var register = typeof(AuthController).GetMethod(nameof(AuthController.Register));
        var login = typeof(AuthController).GetMethod(nameof(AuthController.Login));

        Assert.NotNull(register);
        Assert.NotNull(login);
        Assert.NotNull(register!.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).SingleOrDefault());
        Assert.NotNull(login!.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).SingleOrDefault());
    }

    [Fact]
    public void UsersController_RequiresAuthentication()
    {
        var authorize = typeof(UsersController)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .SingleOrDefault();

        Assert.NotNull(authorize);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Issuer"] = "UserManager",
                ["JwtSettings:Audience"] = "UserManager.Tests",
                ["JwtSettings:SecretKey"] = SecretKey,
                ["JwtSettings:ExpireMinutes"] = "60"
            })
            .Build();
    }
}
