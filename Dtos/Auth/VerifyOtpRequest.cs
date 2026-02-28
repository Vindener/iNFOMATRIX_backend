namespace Infomatrix.Api.Dtos.Auth;

public record VerifyOtpRequest(
    string Email,
    string Token);
