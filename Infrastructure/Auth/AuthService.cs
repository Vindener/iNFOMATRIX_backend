using SupabaseClient = Supabase.Client;

using Infomatrix.Api.Abstractions.Services;
using Infomatrix.Api.Domain;
using Infomatrix.Api.Dtos;
using Microsoft.Extensions.Options;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using System.Text.Json;

namespace StarterApp.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly string _secretKey;
    private readonly SupabaseClient _supabaseClient;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        SupabaseClient supabaseClient,
        ILogger<AuthService> logger,
        IOptionsSnapshot<SupabaseOptions> options)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
        _secretKey = options.Value.AdminKey;
    }

    public async Task<string> RegisterUserAsync(string email, string password)
    {
        try
        {
            var session = await _supabaseClient.Auth.SignUp(email, password);

            if (session?.User is null)
                AuthenticationException.ThrowUnknownError();

            if (IsFakeUser(session.User))
                AuthenticationException.ThrowEmailAlreadyExists(email);

            return email;
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    public async Task<TokenDto> ConfirmEmailAsync(
        string email,
        string token,
        string lastPassword)
    {
        if (string.IsNullOrEmpty(lastPassword))
        {
            AuthenticationException.ThrowTokenExpired();
        }

        try
        {
            var session = await _supabaseClient.Auth
                .VerifyOTP(email, token, Constants.EmailOtpType.Email);

            if (session is null)
            {
                LogNullSession(nameof(ConfirmEmailAsync), $"Email: {email}");
                AuthenticationException.ThrowNullSession();
            }

            await TryUpdatePasswordAsync(lastPassword);

            return MapToTokenDto(session);
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    public async Task<TokenDto> LoginAsync(string email, string password)
    {
        try
        {
            var session = await _supabaseClient.Auth
                .SignIn(email, password);

            if (session is null)
            {
                LogNullSession(nameof(LoginAsync), $"Email: {email}");
                AuthenticationException.ThrowNullSession();
            }

            return MapToTokenDto(session);
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    public Task<TokenDto> LoginWithGoogleAsync(string idToken) =>
        LoginWithProviderAsync(idToken, Constants.Provider.Google);

    public Task<TokenDto> LoginWithAppleAsync(string idToken) =>
        LoginWithProviderAsync(idToken, Constants.Provider.Apple);

    private async Task<TokenDto> LoginWithProviderAsync(
        string idToken,
        Constants.Provider provider)
    {
        try
        {
            var session = await _supabaseClient.Auth.SignInWithIdToken(
                provider: provider,
                idToken: idToken);

            if (session is null)
            {
                LogNullSession(nameof(LoginWithProviderAsync), $"Provider: {provider}");
                AuthenticationException.ThrowOAuthProviderError(provider.ToString());
            }

            return MapToTokenDto(session);
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    public async Task<TokenDto> RefreshTokenAsync(
        string accessToken,
        string refreshToken)
    {
        try
        {
            var session = await _supabaseClient.Auth
                .SetSession(accessToken, refreshToken, false);

            session = await _supabaseClient.Auth
                .RefreshSession();

            if (session is null)
            {
                LogNullSession(nameof(RefreshTokenAsync));
                AuthenticationException.ThrowInvalidToken();
            }

            return MapToTokenDto(session);
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    public async Task<string> RequestResetPasswordAsync(string email)
    {
        try
        {
            await _supabaseClient.Auth
                .ResetPasswordForEmail(email);

            return email;
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    public async Task<TokenDto> VerifyOtpAsync(string email, string token)
    {
        try
        {
            var session = await _supabaseClient.Auth.VerifyOTP(
                email,
                token,
                Constants.EmailOtpType.Recovery);

            if (session is null)
            {
                LogNullSession(nameof(VerifyOtpAsync), $"Email: {email}");
                AuthenticationException.ThrowInvalidToken();
            }

            return MapToTokenDto(session);
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    public async Task<TokenDto> ResetPasswordAsync(
        string email,
        string newPassword,
        TokenDto tokenDto)
    {
        try
        {
            var session = await _supabaseClient.Auth.SetSession(
                tokenDto.AccessToken,
                tokenDto.RefreshToken);

            if (session is null)
            {
                LogNullSession(nameof(ResetPasswordAsync), $"Email: {email}");
                AuthenticationException.ThrowInvalidToken();
            }

            await TryUpdatePasswordAsync(newPassword);

            return MapToTokenDto(session);
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    public async Task DeleteAccountAsync(string id)
    {
        try
        {
            var adminAuthClient = _supabaseClient
                .AdminAuth(_secretKey);

            var user = await adminAuthClient
                .GetUserById(id);

            if (user is null)
            {
                AuthenticationException.ThrowUserNotFound(id);
            }

            await adminAuthClient
                .DeleteUser(id);
        }
        catch (GotrueException ex)
        {
            HandleGoTrueException(ex);
            throw;
        }
    }

    private async Task TryUpdatePasswordAsync(string password)
    {
        try
        {
            await _supabaseClient.Auth.Update(new UserAttributes
            {
                Password = password
            });
        }
        catch (GotrueException ex)
        {
            var error = JsonSerializer
                .Deserialize<SupabaseError>(ex.Message);

            if (error == null || !error.ErrorCode.Contains("same_password"))
            {
                throw;
            }
        }
    }

    private void LogNullSession(string context, string? details = null)
    {
        _logger.LogWarning(
            "Session is null. Context: {Context}, Details: {Details}",
            context,
            details ?? "Without any details");
    }

    private static TokenDto MapToTokenDto(Session session)
    {
        if (session?.AccessToken is null || session.RefreshToken is null)
            AuthenticationException.ThrowNullSession();

        return new TokenDto(
            session.AccessToken!,
            session.RefreshToken!,
            session.ExpiresIn,
            session.TokenType ?? "bearer");
    }

    private static bool IsFakeUser(User user)
    {
        return user?.Email?.Contains("fakeemail", StringComparison.OrdinalIgnoreCase) == true;
    }

    private void HandleGoTrueException(GotrueException ex)
    {
        _logger.LogError(ex, "Supabase Gotrue error occurred");

        var error = TryDeserializeError(ex.Message);

        if (error != null)
        {
            throw error.ErrorCode.ToLowerInvariant() switch
            {
                var code when code.Contains("invalid") => new AuthenticationException(error.Message),
                var code when code.Contains("not_found") => new AuthenticationException(error.Message, StatusCodes.Status404NotFound),
                var code when code.Contains("already_exists") || code.Contains("duplicate") =>
                    new AuthenticationException(error.Message, StatusCodes.Status409Conflict),
                var code when code.Contains("expired") => new AuthenticationException("Token has expired."),
                _ => new AuthenticationException(error.Message)
            };
        }

        throw new AuthenticationException(ex.Message);
    }

    private SupabaseError? TryDeserializeError(string message)
    {
        try
        {
            return JsonSerializer.Deserialize<SupabaseError>(message);
        }
        catch
        {
            return null;
        }
    }
}
