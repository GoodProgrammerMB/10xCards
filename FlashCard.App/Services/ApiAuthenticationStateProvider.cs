using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace FlashCard.App.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ILogger<ApiAuthenticationStateProvider> _logger;
    private readonly IJSRuntime _jsRuntime;
    private bool _isInitialized;

    public ApiAuthenticationStateProvider(
        ProtectedLocalStorage protectedLocalStorage,
        ILogger<ApiAuthenticationStateProvider> logger,
        IJSRuntime jsRuntime)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _logger = logger;
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        try
        {
            if (!_isInitialized)
            {
                try
                {
                    await _jsRuntime.InvokeAsync<bool>("window.hasOwnProperty", "localStorage");
                    _isInitialized = true;
                }
                catch (JSException)
                {
                    _logger.LogInformation("JS Runtime not available - returning anonymous state");
                    return anonymous;
                }
            }

            var tokenResult = await _protectedLocalStorage.GetAsync<string>("authToken");
            if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value))
            {
                return anonymous;
            }

            var userResult = await _protectedLocalStorage.GetAsync<FlashCard.App.Models.UserDto>("user");
            if (!userResult.Success || userResult.Value == null)
            {
                return anonymous;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userResult.Value.Id.ToString()),
                new Claim(ClaimTypes.Email, userResult.Value.Email)
            };

            var identity = new ClaimsIdentity(claims, "apiauth_type");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania stanu autentykacji");
            return anonymous;
        }
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        var userResult = await _protectedLocalStorage.GetAsync<FlashCard.App.Models.UserDto>("user");
        if (!userResult.Success || userResult.Value == null)
        {
            return;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userResult.Value.Id.ToString()),
            new Claim(ClaimTypes.Email, userResult.Value.Email)
        };

        var identity = new ClaimsIdentity(claims, "apiauth_type");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _protectedLocalStorage.DeleteAsync("authToken");
        await _protectedLocalStorage.DeleteAsync("user");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }
} 