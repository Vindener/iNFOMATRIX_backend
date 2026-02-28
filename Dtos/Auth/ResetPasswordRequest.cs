namespace Infomatrix.Dtos.Auth;

public record ResetPasswordRequest(
    string Email,
    string NewPassword,
    string AccessToken,
    string RefreshToken);
