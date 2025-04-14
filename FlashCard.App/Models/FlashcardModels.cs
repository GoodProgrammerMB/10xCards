using System.ComponentModel.DataAnnotations;

namespace FlashCard.App.Models;

public class FlashcardDTO
{
    [Required]
    public string Front { get; set; } = string.Empty;
    [Required]
    public string Back { get; set; } = string.Empty;
    [Required]
    public string Source { get; set; } = string.Empty;
}

public class FlashcardViewModel
{
    public int Id { get; set; }
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool Accepted { get; set; }
    public bool Edited { get; set; }
    public string TemporaryId { get; set; } = string.Empty;
} 