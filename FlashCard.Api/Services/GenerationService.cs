using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FlashCard.Api.Configuration;
using FlashCard.Api.Data;
using FlashCard.Api.Models;
using FlashCard.Api.Services.OpenRouter;
using FlashCard.Api.Services.OpenRouter.Models;
using Microsoft.Extensions.Logging;

namespace FlashCard.Api.Services;

public class GenerationService : IGenerationService
{
    private readonly ILogger<GenerationService> _logger;
    private readonly FlashCardDbContext _dbContext;
    private readonly IOpenRouterService _openRouterService;

    public GenerationService(
        ILogger<GenerationService> logger,
        FlashCardDbContext dbContext,
        IOpenRouterService openRouterService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _openRouterService = openRouterService;
    }

    public async Task<GenerationResponseDto> GenerateFlashcardsAsync(GenerationRequestDto request, int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var generation = new Generation
            {
                UserId = userId,
                Model = request.Model ?? _openRouterService.DefaultModelName,
                SourceTextHash = ComputeHash(request.SourceText),
                GeneratedCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _dbContext.Generations.AddAsync(generation, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var systemMessage = @"You are a helpful AI assistant that creates flashcards from provided text. 
                Create concise and clear flashcards in JSON format. Each flashcard should have 'front' and 'back' fields.
                The front should contain a question or concept, and the back should contain the answer or explanation.
                Return only valid JSON array of flashcards without any additional text.";

            var userMessage = $"Create flashcards from this text: {request.SourceText}";

            var parameters = new Dictionary<string, object>
            {
                { "temperature", 0.7 },
                { "max_tokens", 1000 }
            };

            var responseFormat = new ResponseFormat 
            { 
                Type = "json_object"
            };

            var response = await _openRouterService.SendRequest(
                userMessage,
                systemMessage,
                generation.Model,
                parameters,
                responseFormat,
                cancellationToken);

            var content = await _openRouterService.GetResponseContent(response);
            
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