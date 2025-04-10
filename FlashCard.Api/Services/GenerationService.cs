using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FlashCard.Api.Configuration;
using FlashCard.Api.Data;
using FlashCard.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Threading;

namespace FlashCard.Api.Services;

public class GenerationService : IGenerationService
{
    private readonly ILogger<GenerationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly FlashCardDbContext _dbContext;
    private readonly OpenRouterOptions _options;
    private const string GenerationEndpoint = "/generations";
    
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

    public async Task<GenerationResponseDto> GenerateFlashcardsAsync(GenerationRequestDto request, int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(GenerationEndpoint, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var generation = new Generation
            {
                UserId = userId,
                Model = request.Model ?? "default",
                SourceTextHash = ComputeHash(request.SourceText),
                GeneratedCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _dbContext.Generations.AddAsync(generation, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            var openRouterResponse = await response.Content.ReadFromJsonAsync<OpenRouterResponse>(cancellationToken: cancellationToken);
            var content = openRouterResponse?.Choices?.FirstOrDefault()?.Message?.Content;
            
            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("Empty response from OpenRouter API");
            }
            
            var flashcards = JsonSerializer.Deserialize<List<GenerationFlashcardDto>>(content);
            if (flashcards == null || !flashcards.Any())
            {
                throw new Exception("Failed to parse flashcards from API response");
            }
            
            generation.GeneratedCount = flashcards.Count;
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return new GenerationResponseDto
            {
                Id = generation.Id,
                UserId = userId,
                Model = generation.Model,
                GeneratedCount = generation.GeneratedCount,
                Flashcards = flashcards,
                CreatedAt = generation.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating flashcards for user {UserId}", userId);
            
            await _dbContext.GenerationErrorLogs.AddAsync(new GenerationErrorLog
            {
                UserId = userId,
                ErrorCode = ex.GetType().Name,
                ErrorMessage = ex.Message,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
            
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