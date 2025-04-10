namespace FlashCard.App.Models;

public class BatchSaveResponse
{
    public List<SavedFlashcard> Data { get; set; } = new();
    public BatchSaveSummary Summary { get; set; } = new();
}

public class SavedFlashcard
{
    public int Id { get; set; }
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public int? GenerationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class BatchSaveSummary
{
    public int TotalCreated { get; set; }
    public int TotalFailed { get; set; }
} 