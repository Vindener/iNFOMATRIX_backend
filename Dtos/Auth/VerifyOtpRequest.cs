namespace Infomatrix.Dtos.Auth;

public record VerifyOtpRequest(
    string Email,
    string Token);
