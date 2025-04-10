using FlashCard.Api.Models;

namespace FlashCard.Api.Services;

public interface IGenerationService
{
    Task<GenerationResponseDto> GenerateFlashcardsAsync(GenerationRequestDto request, int userId, CancellationToken cancellationToken = default);
} 