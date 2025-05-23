using Microsoft.JSInterop;
using System.Text.Json;

namespace FlashCard.App.Services;

public interface ILocalStorageService
{
    Task<T> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
}

public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<T> GetItemAsync<T>(string key)
    {
        if (_jsRuntime is IJSInProcessRuntime)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (json == null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
        
        // During prerendering, return default value
        return default;
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        if (_jsRuntime is IJSInProcessRuntime)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        if (_jsRuntime is IJSInProcessRuntime)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
} 