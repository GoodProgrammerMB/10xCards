using System.Net.Http.Json;
using FlashCard.App.Models;

namespace FlashCard.App.Services
{
    public interface IGenerationService
    {
        Task<GenerationResponseDTO> GenerateFlashcardsAsync(GenerationRequestDTO request);
    }

    public class GenerationService : IGenerationService
    {
        private readonly HttpClient _httpClient;
        private const string GenerationEndpoint = "api/generations";

        public GenerationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GenerationResponseDTO> GenerateFlashcardsAsync(GenerationRequestDTO request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(GenerationEndpoint, request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<GenerationResponseDTO>();
            }
            catch (Exception ex)
            {
                // TODO: Add proper error handling and logging
                throw new Exception("Błąd podczas generowania fiszek", ex);
            }
        }
    }
} 