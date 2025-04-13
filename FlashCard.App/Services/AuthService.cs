using System.Net.Http.Json;
using System.Text.Json;
using FlashCard.App.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FlashCard.App.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ApiAuthenticationStateProvider _authStateProvider;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IHttpClientFactory httpClientFactory,
        NavigationManager navigationManager,
        ProtectedLocalStorage protectedLocalStorage,
        ApiAuthenticationStateProvider authStateProvider,
        ILogger<AuthService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _navigationManager = navigationManager;
        _protectedLocalStorage = protectedLocalStorage;
        _authStateProvider = authStateProvider;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest model)
    {
        try
        {
            _logger.LogInformation("Attempting to register user: {Email}", model.Email);
            
            var client = _httpClientFactory.CreateClient("PublicAPI");
            var response = await client.PostAsJsonAsync("api/Users/register", model);
            _logger.LogInformation("Register response status: {StatusCode}", response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Register failed. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                return new AuthResponse { Successful = false, Error = $"Błąd rejestracji: {errorContent}" };
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result?.Successful ?? false)
            {
                await _protectedLocalStorage.SetAsync("authToken", result.Token);
                await _protectedLocalStorage.SetAsync("user", result.User);
                await _authStateProvider.MarkUserAsAuthenticated(result.Token!);
                _logger.LogInformation("User registered successfully: {Email}", model.Email);
            }
            
            return result ?? new AuthResponse { Successful = false, Error = "Nieznany błąd" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during registration");
            return new AuthResponse { Successful = false, Error = $"Błąd rejestracji: {ex.Message}" };
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest model)
    {
        try
        {
            _logger.LogInformation("Attempting to login user: {Email}", model.Email);
            
            var client = _httpClientFactory.CreateClient("PublicAPI");
            var response = await client.PostAsJsonAsync("api/Users/login", model);
            _logger.LogInformation("Login response status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Login failed. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                return new AuthResponse { Successful = false, Error = $"Błąd logowania: {errorContent}" };
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result?.Successful ?? false)
            {
                await _protectedLocalStorage.SetAsync("authToken", result.Token);
                await _protectedLocalStorage.SetAsync("user", result.User);
                await _authStateProvider.MarkUserAsAuthenticated(result.Token!);
                _logger.LogInformation("User logged in successfully: {Email}", model.Email);
            }
            
            return result ?? new AuthResponse { Successful = false, Error = "Nieznany błąd" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login");
            return new AuthResponse { Successful = false, Error = $"Błąd logowania: {ex.Message}" };
        }
    }

    public async Task LogoutAsync()
    {
        await _protectedLocalStorage.DeleteAsync("authToken");
        await _protectedLocalStorage.DeleteAsync("user");
        await _authStateProvider.MarkUserAsLoggedOut();
        _navigationManager.NavigateTo("/logowanie", false);
        _logger.LogInformation("User logged out");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var result = await _protectedLocalStorage.GetAsync<string>("authToken");
            return result.Success && !string.IsNullOrEmpty(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking authentication status");
            return false;
        }
    }
} 