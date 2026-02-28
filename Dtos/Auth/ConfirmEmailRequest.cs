namespace Infomatrix.Api.Dtos.Auth;

public record ConfirmEmailRequest(
    string Email,
    string Token,
    string Password);
