namespace FlashCard.App.Models;

public class Flashcard
{
    public int Id { get; set; }
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool Accepted { get; set; }
    public string TemporaryId { get; set; } = string.Empty;
} 