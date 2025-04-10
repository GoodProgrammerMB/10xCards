using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.JSInterop;

namespace FlashCard.App.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public ApiAuthenticationStateProvider(ILocalStorageService localStorage, IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Check if we're running in prerendering mode
            if (_jsRuntime is IJSInProcessRuntime)
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                
                if (string.IsNullOrEmpty(token))
                {
                    return new AuthenticationState(_anonymous);
                }

                var claims = ParseClaimsFromJwt(token);
                var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                return new AuthenticationState(user);
            }
            else
            {
                // During prerendering, return anonymous user
                return new AuthenticationState(_anonymous);
            }
        }
        catch
        {
            // If any error occurs, return anonymous user
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
} 