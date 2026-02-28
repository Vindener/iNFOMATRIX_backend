using Infomatrix.Api.Dtos;

namespace Infomatrix.Api.Abstractions.Services;

public interface IAuthService
{
    Task<string> RegisterUserAsync(string email, string password);

    Task<TokenDto> ConfirmEmailAsync(string email, string token, string lastPassword);

    Task<TokenDto> LoginAsync(string email, string password);

    Task<TokenDto> LoginWithGoogleAsync(string idToken);

    Task<TokenDto> LoginWithAppleAsync(string idToken);

    Task<TokenDto> RefreshTokenAsync(string accessToken, string refreshToken);

    Task<string> RequestResetPasswordAsync(string email);

    Task<TokenDto> VerifyOtpAsync(string email, string token);

    Task<TokenDto> ResetPasswordAsync(string email, string newPassword, TokenDto tokenDto);

    Task DeleteAccountAsync(string id);
}
