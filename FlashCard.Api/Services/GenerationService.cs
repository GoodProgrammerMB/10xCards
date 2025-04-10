using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FlashCard.Api.Configuration;
using FlashCard.Api.Data;
using FlashCard.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace FlashCard.Api.Services;

public class GenerationService : IGenerationService
{
    private readonly ILogger<GenerationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly FlashCardDbContext _dbContext;
    private readonly OpenRouterOptions _options;
    
    public GenerationService(
        ILogger<GenerationService> logger,
        HttpClient httpClient,
        FlashCardDbContext dbContext,
        IOptions<OpenRouterOptions> options)
    {
        _logger = logger;
        _httpClient = httpClient;
        _dbContext = dbContext;
        _options = options.Value;
        
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<GenerationResponseDto> GenerateFlashcardsAsync(
        GenerationRequestDto request,
        int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sourceTextHash = ComputeHash(request.SourceText);
            var model = request.Model ?? _options.DefaultModel;
            
            // Przygotowanie zapytania do AI
            var aiRequest = new
            {
                model = model,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Jesteś ekspertem w tworzeniu fiszek edukacyjnych. Twoim zadaniem jest analiza tekstu i utworzenie z niego wysokiej jakości fiszek w formacie JSON. Każda fiszka powinna zawierać pole 'front' z pytaniem lub pojęciem i pole 'back' z odpowiedzią lub definicją."
                    },
                    new
                    {
                        role = "user",
                        content = $"Utwórz fiszki z poniższego tekstu:\n\n{request.SourceText}"
                    }
                }
            };

            // Wywołanie API
            var response = await _httpClient.PostAsJsonAsync("/chat", aiRequest, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var aiResponse = await response.Content.ReadFromJsonAsync<OpenRouterResponse>(cancellationToken: cancellationToken);
            var flashcardsJson = aiResponse?.Choices?[0]?.Message?.Content 
                ?? throw new Exception("Invalid AI response format");
            
            var flashcards = JsonSerializer.Deserialize<List<GenerationFlashcardDto>>(flashcardsJson)
                ?? throw new Exception("Could not parse flashcards from AI response");

            // Zapis do bazy danych
            var generation = new Generation
            {
                UserId = userId,
                Model = model,
                SourceTextHash = sourceTextHash,
                GeneratedCount = flashcards.Count,
                Flashcards = flashcards.Select(f => new Flashcard
                {
                    UserId = userId,
                    Front = f.Front,
                    Back = f.Back,
                    Source = f.Source
                }).ToList()
            };

            _dbContext.Generations.Add(generation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Przygotowanie odpowiedzi
            return new GenerationResponseDto
            {
                Id = generation.Id,
                UserId = userId,
                Model = model,
                GeneratedCount = flashcards.Count,
                Flashcards = flashcards,
                CreatedAt = generation.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during flashcard generation for user {UserId}", userId);
            
            // Logowanie błędu do bazy
            var errorLog = new GenerationErrorLog
            {
                UserId = userId,
                Model = request.Model ?? _options.DefaultModel,
                SourceTextHash = ComputeHash(request.SourceText),
                ErrorMessage = ex.Message,
                ErrorDetails = ex.ToString()
            };
            
            _dbContext.GenerationErrorLogs.Add(errorLog);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            throw;
        }
    }
    
    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

// Klasy pomocnicze do deserializacji odpowiedzi z OpenRouter
file class OpenRouterResponse
{
    public List<OpenRouterChoice>? Choices { get; set; }
}

file class OpenRouterChoice
{
    public OpenRouterMessage? Message { get; set; }
}

file class OpenRouterMessage
{
    public string? Content { get; set; }
} 