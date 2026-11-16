namespace UserManager.DTO;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role = "User");

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(
    string AccessToken,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyCollection<string> Roles,
    DateTime ExpiresAt);
