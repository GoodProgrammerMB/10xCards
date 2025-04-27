namespace FlashCard.App.Models;

public class ProcessAnswerRequest
{
    public int FlashcardId { get; set; }
    public bool WasCorrect { get; set; }
} 