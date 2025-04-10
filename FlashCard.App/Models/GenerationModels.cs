using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlashCard.App.Models
{
    public class GenerationRequestDTO
    {
        [Required]
        public string SourceText { get; set; } = string.Empty;
        [Required]
        public string Model { get; set; } = string.Empty;
    }

    public class GenerationResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public string Model { get; set; } = string.Empty;
        public int GeneratedCount { get; set; }
        public List<FlashcardDTO> Flashcards { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FlashcardDTO
    {
        [Required]
        public string Front { get; set; } = string.Empty;
        [Required]
        public string Back { get; set; } = string.Empty;
        [Required]
        public string Source { get; set; } = string.Empty;
    }

    public class FlashcardViewModel : FlashcardDTO
    {
        public int Id { get; set; }
        public bool Accepted { get; set; }
        public bool Edited { get; set; }
        public string TemporaryId { get; set; } = string.Empty;

        public static FlashcardViewModel FromFlashcard(Flashcard flashcard)
        {
            return new FlashcardViewModel
            {
                Id = flashcard.Id,
                Front = flashcard.Front,
                Back = flashcard.Back,
                Source = flashcard.Source,
                Accepted = flashcard.Accepted,
                TemporaryId = flashcard.TemporaryId
            };
        }

        public Flashcard ToFlashcard()
        {
            return new Flashcard
            {
                Id = Id,
                Front = Front,
                Back = Back,
                Source = Source,
                Accepted = Accepted,
                TemporaryId = TemporaryId
            };
        }
    }

    public class GenerationRequest
    {
        [Required]
        public string SourceText { get; set; } = string.Empty;
        [Required]
        public string Model { get; set; } = string.Empty;
    }

    public class GenerationResponse
    {
        [Required]
        public string Model { get; set; } = string.Empty;
        public ICollection<GeneratedFlashcard> Flashcards { get; set; } = new List<GeneratedFlashcard>();
    }

    public class GeneratedFlashcard
    {
        [Required]
        public string Front { get; set; } = string.Empty;
        [Required]
        public string Back { get; set; } = string.Empty;
        [Required]
        public string Source { get; set; } = string.Empty;
        public string TemporaryId { get; set; } = string.Empty;
    }
} 