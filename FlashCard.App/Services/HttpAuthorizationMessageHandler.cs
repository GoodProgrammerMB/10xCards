using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using FlashCard.App.Models;

namespace FlashCard.App.Services;

public class HttpAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorage;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IJSRuntime _jsRuntime;

    public HttpAuthorizationMessageHandler(
        ITokenStorageService tokenStorage, 
        NavigationManager navigationManager,
        AuthenticationStateProvider authStateProvider,
        IJSRuntime jsRuntime)
    {
        _tokenStorage = tokenStorage;
        _navigationManager = navigationManager;
        _authStateProvider = authStateProvider;
        _jsRuntime = jsRuntime;
        Console.WriteLine("[HttpAuthHandler] Initialized");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[HttpAuthHandler] Processing request to {request.RequestUri}");
            Console.WriteLine($"[HttpAuthHandler] Request method: {request.Method}");
            
            var token = await _tokenStorage.GetTokenAsync(SessionKeys.Token);
            Console.WriteLine($"[HttpAuthHandler] Token retrieved: {(token != null ? "Present" : "Null")}");

            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine("[HttpAuthHandler] Adding token to request headers");
                Console.WriteLine($"[HttpAuthHandler] Token preview: {token.Substring(0, Math.Min(20, token.Length))}...");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                Console.WriteLine("[HttpAuthHandler] Request headers:");
                foreach (var header in request.Headers)
                {
                    Console.WriteLine($"[HttpAuthHandler] {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
            else
            {
                Console.WriteLine("[HttpAuthHandler] No token available");
            }

            Console.WriteLine("[HttpAuthHandler] Sending request");
            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine($"[HttpAuthHandler] Response status: {response.StatusCode}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("[HttpAuthHandler] Received unauthorized response");
                await HandleUnauthorizedResponse();
            }

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HttpAuthHandler] Error processing request: {ex.Message}");
            Console.WriteLine($"[HttpAuthHandler] Stack trace: {ex.StackTrace}");
            // W przypadku błędu, zwróć odpowiedź 401 aby zainicjować proces wylogowania
            return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
        }
    }

    private async Task HandleUnauthorizedResponse()
    {
        try
        {
            Console.WriteLine("[HttpAuthHandler] Handling unauthorized response");
            await _tokenStorage.DeleteTokensAsync();
            ((ApiAuthenticationStateProvider)_authStateProvider).NotifyUserLogout();

            // Sprawdź, czy nie jesteśmy w trybie prerenderingu i czy NavigationManager jest dostępny
            if (_jsRuntime is not IJSInProcessRuntime && _navigationManager != null)
            {
                var currentUri = _navigationManager.Uri;
                Console.WriteLine($"[HttpAuthHandler] Current URI: {currentUri}");
                
                if (!currentUri.Contains("/auth"))
                {
                    Console.WriteLine("[HttpAuthHandler] Redirecting to auth page");
                    _navigationManager.NavigateTo("/auth", true);
                }
                else
                {
                    Console.WriteLine("[HttpAuthHandler] Already on auth page, skipping redirect");
                }
            }
            else
            {
                Console.WriteLine("[HttpAuthHandler] Cannot redirect - prerendering or NavigationManager not available");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HttpAuthHandler] Error handling unauthorized response: {ex.Message}");
            Console.WriteLine($"[HttpAuthHandler] Stack trace: {ex.StackTrace}");
        }
    }
} 