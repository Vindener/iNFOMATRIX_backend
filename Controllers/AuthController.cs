using Infomatrix.Abstractions.Services;
using Infomatrix.Dtos;
using Infomatrix.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Infomatrix.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType<EmailDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterRequest request)
    {
        var email = await _authService.RegisterUserAsync(
            request.Email,
            request.Password);

        return Ok(new EmailDto(email));
    }

    [HttpPost("confirm-email")]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmEmailAsync(
        [FromBody] ConfirmEmailRequest request)
    {
        var tokenDto = await _authService.ConfirmEmailAsync(
            request.Email,
            request.Token,
            request.Password);

        return Ok(tokenDto);
    }

    [HttpPost("login")]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequest request)
    {
        var tokenDto = await _authService.LoginAsync(
            request.Email,
            request.Password);

        return Ok(tokenDto);
    }

    [HttpPost("google")]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginWithGoogleAsync(
        [FromBody] IdTokenRequest request)
    {
        var tokenDto = await _authService.LoginWithGoogleAsync(request.IdToken);

        return Ok(tokenDto);
    }

    [HttpPost("apple")]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginWithAppleAsync(
        [FromBody] IdTokenRequest request)
    {
        var tokenDto = await _authService.LoginWithAppleAsync(request.IdToken);

        return Ok(tokenDto);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request)
    {
        var tokenDto = await _authService.RefreshTokenAsync(
            request.AccessToken,
            request.RefreshToken);

        return Ok(tokenDto);
    }

    [HttpPost("request-reset-password")]
    [ProducesResponseType<EmailDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestResetPasswordAsync(
        [FromBody] RequestResetPasswordRequest request)
    {
        var email = await _authService.RequestResetPasswordAsync(request.Email);

        return Ok(new EmailDto(email));
    }

    [HttpPost("verify-otp")]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyOtpAsync(
        [FromBody] VerifyOtpRequest request)
    {
        var tokenDto = await _authService.VerifyOtpAsync(
            request.Email,
            request.Token);

        return Ok(tokenDto);
    }

    [HttpPost("reset-password")]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPasswordAsync(
        [FromBody] ResetPasswordRequest request)
    {
        var tokenDto = await _authService.ResetPasswordAsync(
            request.Email,
            request.NewPassword,
            new TokenDto(
                request.AccessToken,
                request.RefreshToken,
                0,
                "bearer"));

        return Ok(tokenDto);
    }
}
