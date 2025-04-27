using FlashCard.Api.Data;
using FlashCard.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashCard.Api.Services;

public class LearningService : ILearningService
{
    private readonly FlashCardDbContext _context;
    private const int SessionSize = 5; // Max 5 cards per session

    public LearningService(FlashCardDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FlashcardDTO>> GetFlashcardsForSessionAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;

        var flashcardsQuery = _context.Flashcards
            .Where(f => f.UserId == userId);

        // 1. Prioritize never reviewed cards
        var neverReviewed = await flashcardsQuery
            .Where(f => f.LastReviewedAt == null)
            .OrderBy(f => Guid.NewGuid()) // Randomize within the group
            .Take(SessionSize)
            .ToListAsync();

        int needed = SessionSize - neverReviewed.Count;
        if (needed <= 0) return MapToDTO(neverReviewed);

        var sessionCards = new List<Flashcard>(neverReviewed);

        // 2. Prioritize cards due today or overdue
        var dueCards = await flashcardsQuery
            .Where(f => f.NextReviewAt != null && f.NextReviewAt.Value.Date <= today)
            .Where(f => !sessionCards.Select(sc => sc.Id).Contains(f.Id)) // Exclude already added
            .OrderBy(f => Guid.NewGuid())
            .Take(needed)
            .ToListAsync();

        sessionCards.AddRange(dueCards);
        needed = SessionSize - sessionCards.Count;
        if (needed <= 0) return MapToDTO(sessionCards);

        // 3. If still need more, take random learned cards (not necessarily due)
        //    For simplicity now, just take any other cards
        var otherCards = await flashcardsQuery
            .Where(f => !sessionCards.Select(sc => sc.Id).Contains(f.Id))
            .OrderBy(f => Guid.NewGuid())
            .Take(needed)
            .ToListAsync();
            
        sessionCards.AddRange(otherCards);

        // Final shuffle of the selected cards
        var random = new Random();
        var shuffledSession = sessionCards.OrderBy(item => random.Next()).ToList();

        return MapToDTO(shuffledSession);
    }

    public async Task<bool> ProcessAnswerAsync(int userId, int flashcardId, bool wasCorrect)
    {
        var flashcard = await _context.Flashcards
            .FirstOrDefaultAsync(f => f.Id == flashcardId && f.UserId == userId);

        if (flashcard == null)
        {
            return false; // Or throw exception
        }

        flashcard.LastReviewedAt = DateTime.UtcNow;
        flashcard.IsLearned = true;

        if (wasCorrect)
        {
            flashcard.CorrectAnswersInRow++;
            // Simple logic: add days based on correct answers in row (1, 2, 4, 8... capped at 30?)
            int daysToAdd = (int)Math.Min(30, Math.Pow(2, flashcard.CorrectAnswersInRow -1)); // Start with 1 day for the first correct answer
            flashcard.NextReviewAt = DateTime.UtcNow.Date.AddDays(daysToAdd);
        }
        else
        {
            flashcard.CorrectAnswersInRow = 0;
            flashcard.NextReviewAt = DateTime.UtcNow.Date.AddDays(1); // Review again tomorrow
        }

        flashcard.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private IEnumerable<FlashcardDTO> MapToDTO(IEnumerable<Flashcard> flashcards)
    {
        return flashcards.Select(f => new FlashcardDTO
        {
            Id = f.Id,
            Front = f.Front,
            Back = f.Back,
            Source = f.Source
            // Don't expose learning details in the basic DTO for session retrieval
        });
    }
} 