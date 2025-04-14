using FlashCard.App.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace FlashCard.App.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterModel model);
    Task<AuthResponse> LoginAsync(LoginModel model);
    Task Logout();
    Task<bool> IsAuthenticated();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        ITokenStorageService tokenStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", model);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result?.Successful ?? false)
        {
            await _tokenStorage.SetTokenAsync(SessionKeys.Token, result.Token!);
            await _tokenStorage.SetRefreshTokenAsync(SessionKeys.RefreshToken, result.Token!);
            ((ApiAuthenticationStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token!);
        }
        
        return result ?? new AuthResponse { Successful = false, Error = "Nieznany błąd" };
    }

    public async Task<AuthResponse> LoginAsync(LoginModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result?.Successful ?? false)
        {
            await _tokenStorage.SetTokenAsync(SessionKeys.Token, result.Token!);
            await _tokenStorage.SetRefreshTokenAsync(SessionKeys.RefreshToken, result.Token!);
            ((ApiAuthenticationStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token!);
        }
        
        return result ?? new AuthResponse { Successful = false, Error = "Nieznany błąd" };
    }

    public async Task Logout()
    {
        await _tokenStorage.DeleteTokensAsync();
        ((ApiAuthenticationStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<bool> IsAuthenticated()
    {
        var token = await _tokenStorage.GetTokenAsync(SessionKeys.Token);
        return !string.IsNullOrEmpty(token);
    }
} 