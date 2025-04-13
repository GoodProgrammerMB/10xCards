namespace FlashCard.App.Models;

public class GenerationResponseDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Model { get; set; } = string.Empty;
    public int GeneratedCount { get; set; }
    public List<FlashcardDTO> Flashcards { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class FlashcardDTO
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
} 