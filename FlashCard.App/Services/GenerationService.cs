using System.Net.Http.Json;
using FlashCard.App.Models;
using Microsoft.Extensions.Logging;

namespace FlashCard.App.Services;

public class GenerationService : IGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GenerationService> _logger;

    public GenerationService(IHttpClientFactory httpClientFactory, ILogger<GenerationService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("AuthAPI");
        _logger = logger;
    }

    public async Task<GenerationResponseDTO> GenerateFlashcardsAsync(GenerationRequestDTO request)
    {
        try
        {
            _logger.LogInformation("Wysyłanie żądania generowania fiszek");
            var response = await _httpClient.PostAsJsonAsync("/api/generations", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Błąd podczas generowania fiszek: {StatusCode} - {Error}", 
                    response.StatusCode, error);
                throw new Exception($"Błąd podczas generowania fiszek: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<GenerationResponseDTO>();
            if (result == null)
            {
                throw new Exception("Otrzymano pustą odpowiedź z API");
            }

            _logger.LogInformation("Pomyślnie wygenerowano {Count} fiszek", result.Flashcards.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas generowania fiszek");
            throw;
        }
    }
} 