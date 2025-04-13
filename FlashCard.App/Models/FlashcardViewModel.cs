using System.ComponentModel.DataAnnotations;

namespace FlashCard.App.Models;

public class FlashcardViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Front fiszki jest wymagany")]
    [StringLength(500, ErrorMessage = "Front fiszki nie może być dłuższy niż 500 znaków")]
    public string Front { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Tył fiszki jest wymagany")]
    [StringLength(1000, ErrorMessage = "Tył fiszki nie może być dłuższy niż 1000 znaków")]
    public string Back { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;
    
    public int DeckId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Accepted { get; set; } = false;
    public bool Edited { get; set; } = false;
} 