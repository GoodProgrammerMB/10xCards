using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace FlashCard.App.Services;

public class HttpAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ILogger<HttpAuthorizationMessageHandler> _logger;
    private readonly IJSRuntime _jsRuntime;
    private string _cachedToken = string.Empty;

    public HttpAuthorizationMessageHandler(
        ProtectedLocalStorage protectedLocalStorage,
        ILogger<HttpAuthorizationMessageHandler> logger,
        IJSRuntime jsRuntime)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _logger = logger;
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting request to {RequestUri}", request.RequestUri);
        _logger.LogInformation("Request headers before auth: {Headers}", 
            string.Join(", ", request.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));

        try
        {
            // Sprawdź, czy jesteśmy w przeglądarce
            var isBrowser = false;
            try
            {
                isBrowser = await _jsRuntime.InvokeAsync<bool>("window.hasOwnProperty", "localStorage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking browser context, waiting...");
                await Task.Delay(100);
                try
                {
                    isBrowser = await _jsRuntime.InvokeAsync<bool>("window.hasOwnProperty", "localStorage");
                }
                catch (Exception)
                {
                    _logger.LogWarning("Still not in browser context, skipping token check");
                    return await base.SendAsync(request, cancellationToken);
                }
            }

            if (!isBrowser)
            {
                _logger.LogWarning("Not in browser context, skipping token check");
                return await base.SendAsync(request, cancellationToken);
            }

            string token = _cachedToken;
            if (string.IsNullOrEmpty(token))
            {
                try
                {
                    var result = await _protectedLocalStorage.GetAsync<string>("authToken");
                    if (!result.Success || string.IsNullOrEmpty(result.Value))
                    {
                        _logger.LogWarning("No token found in protected storage");
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }

                    token = result.Value;
                    _logger.LogInformation("Token retrieved from protected storage");
                    
                    // Sprawdź ważność tokenu
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    
                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        _logger.LogWarning("Token has expired at {ExpiryTime}", jwtToken.ValidTo);
                        await HandleTokenExpiration();
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }

                    _cachedToken = token;
                }
                catch (CryptographicException ex)
                {
                    _logger.LogError(ex, "Error decrypting token from protected storage");
                    await HandleTokenExpiration();
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving token from protected storage");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation("Authorization header added");
                
                _logger.LogInformation("Request headers after auth: {Headers}", 
                    string.Join(", ", request.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));
            }

            var response = await base.SendAsync(request, cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Received unauthorized response");
                await HandleTokenExpiration();
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendAsync: {Error}", ex.Message);
            throw;
        }
    }

    private async Task HandleTokenExpiration()
    {
        _cachedToken = null;
        try
        {
            await _protectedLocalStorage.DeleteAsync("authToken");
            _logger.LogInformation("Token removed from protected storage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing token from protected storage");
        }
    }
} 