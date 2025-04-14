using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlashCard.Api.Models;

public class GenerationRequestDto
{
    [Required]
    [StringLength(10000, MinimumLength = 1000)]
    public string SourceText { get; set; } = string.Empty;
    
    public string? Model { get; set; }
}

public class GenerationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Model { get; set; } = string.Empty;
    public int GeneratedCount { get; set; }
    public List<GenerationFlashcardDto> Flashcards { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class GenerationFlashcardDto
{
    [JsonPropertyName("front")]
    public string Front { get; set; } = string.Empty;

    [JsonPropertyName("back")]
    public string Back { get; set; } = string.Empty;
} 