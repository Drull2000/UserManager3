using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManager.DTO;
using UserManager.Models;
using UserManager.Services;

namespace UserManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    ITokenService tokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(new
            {
                errors = createResult.Errors.Select(error => error.Description)
            });
        }

        var roleName = user.Role;
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return BadRequest(new
                {
                    errors = roleResult.Errors.Select(error => error.Description)
                });
            }
        }

        var addToRoleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!addToRoleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return BadRequest(new
            {
                errors = addToRoleResult.Errors.Select(error => error.Description)
            });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "User registered successfully",
            userId = user.Id,
            role = roleName
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var roles = await userManager.GetRolesAsync(user);
        var (token, expiresAt) = tokenService.CreateToken(user, roles);

        return Ok(new AuthResponse(
            token,
            user.Email!,
            user.FirstName,
            user.LastName,
            roles.ToArray(),
            expiresAt));
    }
}
