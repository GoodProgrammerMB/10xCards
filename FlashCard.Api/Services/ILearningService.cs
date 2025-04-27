using FlashCard.Api.Models;

namespace FlashCard.Api.Services;

public interface ILearningService
{
    Task<IEnumerable<FlashcardDTO>> GetFlashcardsForSessionAsync(int userId);
    Task<bool> ProcessAnswerAsync(int userId, int flashcardId, bool wasCorrect);
} 