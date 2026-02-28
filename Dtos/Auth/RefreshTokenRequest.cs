namespace Infomatrix.Api.Dtos.Auth;

public record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken);
