using System.ComponentModel.DataAnnotations;

namespace FlashCard.Api.Models;

public class ProcessAnswerRequest
{
    [Required]
    public int FlashcardId { get; set; }

    [Required]
    public bool WasCorrect { get; set; }
} 