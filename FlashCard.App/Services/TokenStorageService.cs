using FlashCard.App.Models;
using Microsoft.JSInterop;

namespace FlashCard.App.Services;

public interface ITokenStorageService
{
    Task<string?> GetTokenAsync(SessionKeys key);
    Task SetTokenAsync(SessionKeys key, string token);
    Task SetRefreshTokenAsync(SessionKeys key, string refreshToken);
    Task DeleteTokensAsync();
}

public class TokenStorageService : ITokenStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private const string TokenPrefix = "flashcard.auth";
    private static string? _cachedToken;
    private static string? _cachedRefreshToken;

    public TokenStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        Console.WriteLine("[TokenStorageService] Initialized");
        Console.WriteLine($"[TokenStorageService] Initial cache state - Token: {(_cachedToken != null ? "Present" : "Null")}, RefreshToken: {(_cachedRefreshToken != null ? "Present" : "Null")}");
    }

    public async Task<string?> GetTokenAsync(SessionKeys key)
    {
        try
        {
            Console.WriteLine($"[TokenStorageService] Getting {key} token");
            Console.WriteLine($"[TokenStorageService] Cache state - Token: {(_cachedToken != null ? "Present" : "Null")}, RefreshToken: {(_cachedRefreshToken != null ? "Present" : "Null")}");

            // Najpierw sprawdź cache
            if (key == SessionKeys.Token && _cachedToken != null)
            {
                Console.WriteLine("[TokenStorageService] Returning cached token");
                return _cachedToken;
            }
            if (key == SessionKeys.RefreshToken && _cachedRefreshToken != null)
            {
                Console.WriteLine("[TokenStorageService] Returning cached refresh token");
                return _cachedRefreshToken;
            }

            // Jeśli nie ma w cache, spróbuj pobrać z ciasteczek
            try
            {
                if (_jsRuntime is IJSInProcessRuntime)
                {
                    Console.WriteLine("[TokenStorageService] JSRuntime is available, trying to get from cookies");
                    var name = $"{TokenPrefix}.{Enum.GetName(typeof(SessionKeys), key)}";
                    var token = await _jsRuntime.InvokeAsync<string>("cookieHelper.getCookie", name);
                    
                    Console.WriteLine($"[TokenStorageService] Cookie value: {(token != null ? "Present" : "Null")}");
                    
                    // Zaktualizuj cache jeśli token został znaleziony
                    if (!string.IsNullOrEmpty(token))
                    {
                        if (key == SessionKeys.Token)
                        {
                            _cachedToken = token;
                            Console.WriteLine("[TokenStorageService] Updated cache with token from cookie");
                        }
                        else if (key == SessionKeys.RefreshToken)
                        {
                            _cachedRefreshToken = token;
                            Console.WriteLine("[TokenStorageService] Updated cache with refresh token from cookie");
                        }
                        return token;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TokenStorageService] Error accessing cookies: {ex.Message}");
            }
            
            Console.WriteLine("[TokenStorageService] Returning from cache");
            // Zwróć z cache (nawet jeśli null)
            return key == SessionKeys.Token ? _cachedToken : _cachedRefreshToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TokenStorageService] Error getting token: {ex.Message}");
            Console.WriteLine($"[TokenStorageService] Stack trace: {ex.StackTrace}");
            return key == SessionKeys.Token ? _cachedToken : _cachedRefreshToken;
        }
    }

    public async Task SetTokenAsync(SessionKeys key, string token)
    {
        try
        {
            Console.WriteLine($"[TokenStorageService] Setting {key} token");
            Console.WriteLine($"[TokenStorageService] Token length: {token.Length}");
            Console.WriteLine($"[TokenStorageService] Token preview: {token.Substring(0, Math.Min(20, token.Length))}...");

            // Zawsze aktualizuj cache
            _cachedToken = token;
            Console.WriteLine("[TokenStorageService] Updated token in cache");

            // Spróbuj zapisać w ciasteczkach jeśli możliwe
            try
            {
                if (_jsRuntime is IJSInProcessRuntime)
                {
                    Console.WriteLine("[TokenStorageService] JSRuntime is available, saving to cookies");
                    var name = $"{TokenPrefix}.{Enum.GetName(typeof(SessionKeys), key)}";
                    await _jsRuntime.InvokeVoidAsync("cookieHelper.setCookie", name, token, 300); // 5 minutes
                    Console.WriteLine("[TokenStorageService] Token saved to cookies");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TokenStorageService] Error saving to cookies: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TokenStorageService] Error setting token: {ex.Message}");
            Console.WriteLine($"[TokenStorageService] Stack trace: {ex.StackTrace}");
        }
    }

    public async Task SetRefreshTokenAsync(SessionKeys key, string refreshToken)
    {
        try
        {
            Console.WriteLine($"[TokenStorageService] Setting refresh token");
            Console.WriteLine($"[TokenStorageService] Refresh token length: {refreshToken.Length}");
            Console.WriteLine($"[TokenStorageService] Refresh token preview: {refreshToken.Substring(0, Math.Min(20, refreshToken.Length))}...");

            // Zawsze aktualizuj cache
            _cachedRefreshToken = refreshToken;
            Console.WriteLine("[TokenStorageService] Updated refresh token in cache");

            // Spróbuj zapisać w ciasteczkach jeśli możliwe
            try
            {
                if (_jsRuntime is IJSInProcessRuntime)
                {
                    Console.WriteLine("[TokenStorageService] JSRuntime is available, saving refresh token to cookies");
                    var name = $"{TokenPrefix}.{Enum.GetName(typeof(SessionKeys), key)}";
                    await _jsRuntime.InvokeVoidAsync("cookieHelper.setCookie", name, refreshToken, 1800); // 30 minutes
                    Console.WriteLine("[TokenStorageService] Refresh token saved to cookies");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TokenStorageService] Error saving to cookies: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TokenStorageService] Error setting refresh token: {ex.Message}");
            Console.WriteLine($"[TokenStorageService] Stack trace: {ex.StackTrace}");
        }
    }

    public async Task DeleteTokensAsync()
    {
        try
        {
            Console.WriteLine("[TokenStorageService] Deleting all tokens");

            // Zawsze czyść cache
            _cachedToken = null;
            _cachedRefreshToken = null;
            Console.WriteLine("[TokenStorageService] Cleared token cache");

            // Spróbuj usunąć ciasteczka jeśli możliwe
            try
            {
                if (_jsRuntime is IJSInProcessRuntime)
                {
                    Console.WriteLine("[TokenStorageService] JSRuntime is available, deleting cookies");
                    var token = $"{TokenPrefix}.{Enum.GetName(typeof(SessionKeys), SessionKeys.Token)}";
                    var refreshToken = $"{TokenPrefix}.{Enum.GetName(typeof(SessionKeys), SessionKeys.RefreshToken)}";

                    await _jsRuntime.InvokeVoidAsync("cookieHelper.eraseCookie", token);
                    await _jsRuntime.InvokeVoidAsync("cookieHelper.eraseCookie", refreshToken);
                    Console.WriteLine("[TokenStorageService] Deleted token cookies");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TokenStorageService] Error deleting cookies: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TokenStorageService] Error deleting tokens: {ex.Message}");
            Console.WriteLine($"[TokenStorageService] Stack trace: {ex.StackTrace}");
        }
    }
} 