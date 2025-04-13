using System.Net.Http.Json;
using FlashCard.Client.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FlashCard.Client.Services;

public interface IGenerationService
{
    Task<GenerationResponse> GenerateFlashcardsAsync(GenerationRequest request);
}

public class GenerationService : IGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GenerationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<GenerationResponse> GenerateFlashcardsAsync(GenerationRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/generations", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<GenerationResponse>();
            if (result == null)
            {
                throw new Exception("Nie udało się przetworzyć odpowiedzi z serwera.");
            }
            
            return result;
        }
        catch (AccessTokenNotAvailableException ex)
        {
            ex.Redirect();
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Błąd podczas generowania fiszek: {ex.Message}", ex);
        }
    }
} 