using System.Net.Http.Json;
using System.Text.Json;
using FlashCard.App.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Users/register", model);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result?.Successful ?? false)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("user", result.User);
            ((ApiAuthenticationStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token!);
        }
        
        return result ?? new AuthResponse { Successful = false, Error = "Nieznany błąd" };
    }

    public async Task<AuthResponse> LoginAsync(LoginModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Users/login", model);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result?.Successful ?? false)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("user", result.User);
            ((ApiAuthenticationStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token!);
        }
        
        return result ?? new AuthResponse { Successful = false, Error = "Nieznany błąd" };
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("user");
        ((ApiAuthenticationStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<bool> IsAuthenticated()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        return !string.IsNullOrEmpty(token);
    }
} 