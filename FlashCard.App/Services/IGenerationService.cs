using FlashCard.App.Models;

namespace FlashCard.App.Services;

public interface IGenerationService
{
    Task<GenerationResponseDTO> GenerateFlashcardsAsync(GenerationRequestDTO request);
} 