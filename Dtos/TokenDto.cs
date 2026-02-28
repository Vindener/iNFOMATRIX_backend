namespace Infomatrix.Api.Dtos;

public record TokenDto(
    string AccessToken,
    string RefreshToken,
    long ExpiresIn,
    string TokenType);
