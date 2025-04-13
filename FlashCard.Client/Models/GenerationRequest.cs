namespace FlashCard.Client.Models;

public class GenerationRequest
{
    public string SourceText { get; set; } = string.Empty;
    public string? Model { get; set; }
}

public class GenerationResponse
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
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
} 