namespace Infomatrix.Api.Dtos.Auth;

public record RegisterRequest(
    string Email,
    string Password);
