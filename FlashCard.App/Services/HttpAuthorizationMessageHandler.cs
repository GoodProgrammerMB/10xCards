using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace FlashCard.App.Services;

public class HttpAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;

    public HttpAuthorizationMessageHandler(ILocalStorageService localStorage, NavigationManager navigationManager)
    {
        _localStorage = localStorage;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _localStorage.RemoveItemAsync("authToken");
            _navigationManager.NavigateTo("/login", true);
        }

        return response;
    }
} 