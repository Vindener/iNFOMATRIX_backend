namespace Infomatrix.Dtos.Auth;

public record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken);
