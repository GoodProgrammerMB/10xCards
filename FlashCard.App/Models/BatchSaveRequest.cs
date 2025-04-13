namespace FlashCard.App.Models
{
    public class BatchSaveRequest
    {
        public List<FlashcardToSave> Flashcards { get; set; } = new();
    }

    public class FlashcardToSave
    {
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public int GenerationId { get; set; }
    }
} 