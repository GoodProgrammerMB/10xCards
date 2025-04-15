using System.Net.Http.Json;
using FlashCard.App.Models;

namespace FlashCard.App.Services;

public interface IGenerationService
{
    Task<GenerationResponseDto> GenerateFlashcardsAsync(GenerationRequest request);
    Task<BatchSaveResponse> SaveFlashcardsAsync(BatchSaveRequest request);
}

public class GenerationService : IGenerationService
{
    private readonly HttpClient _httpClient;

    public GenerationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GenerationResponseDto> GenerateFlashcardsAsync(GenerationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/generations", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GenerationResponseDto>() 
            ?? throw new Exception("Nie udało się przetworzyć odpowiedzi z serwera");
    }

    public async Task<BatchSaveResponse> SaveFlashcardsAsync(BatchSaveRequest request)
    {
        // Przekształć dane do formatu oczekiwanego przez API
        var apiRequest = new
        {
            Flashcards = request.Flashcards.Select(f => new 
            {
                Front = f.FrontDisplay,
                Back = f.BackDisplay,
                GenerationId = f.GenerationId
            }).ToList()
        };

        var response = await _httpClient.PostAsJsonAsync("api/flashcards/batch", apiRequest);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BatchSaveResponse>() 
            ?? throw new Exception("Nie udało się przetworzyć odpowiedzi z serwera");
    }
} 