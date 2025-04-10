using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlashCard.Api.Models
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
        public List<FlashcardDTO> Flashcards { get; set; } = new List<FlashcardDTO>();
        public DateTime CreatedAt { get; set; }
    }

    public class FlashcardDTO
    {
        public int Id { get; set; }
        [Required]
        public string Front { get; set; } = string.Empty;
        [Required]
        public string Back { get; set; } = string.Empty;
        [Required]
        public string Source { get; set; } = string.Empty;
    }

    public class FlashcardViewModel : FlashcardDTO
    {
        public bool? Accepted { get; set; }
        public bool? Edited { get; set; }
        public string TemporaryId { get; set; } = string.Empty;
    }
} 