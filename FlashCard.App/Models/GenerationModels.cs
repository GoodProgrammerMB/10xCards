using System.ComponentModel.DataAnnotations;

namespace FlashCard.App.Models;

public class GenerationRequest
{
    [Required]
    public string SourceText { get; set; } = string.Empty;
    [Required]
    public string Model { get; set; } = string.Empty;
}

public class GenerationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Model { get; set; } = string.Empty;
    public int GeneratedCount { get; set; }
    public List<GeneratedFlashcard> Flashcards { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class GeneratedFlashcard
{
    [Required]
    public string Front { get; set; } = string.Empty;
    [Required]
    public string Back { get; set; } = string.Empty;
    public int? GenerationId { get; set; }
}

public class BatchSaveRequest
{
    public List<GeneratedFlashcard> Flashcards { get; set; } = new();
} 