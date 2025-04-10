using System.Net.Http.Json;
using FlashCard.App.Models;

namespace FlashCard.App.Services;

public interface IFlashcardService
{
    Task<List<Flashcard>> GetUserFlashcardsAsync();
    Task UpdateFlashcardAsync(int id, Flashcard flashcard);
    Task DeleteFlashcardAsync(int id);
}

public class FlashcardService : IFlashcardService
{
    private readonly HttpClient _httpClient;

    public FlashcardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Flashcard>> GetUserFlashcardsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Flashcard>>("api/Flashcards") ?? new List<Flashcard>();
    }

    public async Task UpdateFlashcardAsync(int id, Flashcard flashcard)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Flashcards/{id}", flashcard);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteFlashcardAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/Flashcards/{id}");
        response.EnsureSuccessStatusCode();
    }
} 