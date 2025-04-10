using System.ComponentModel.DataAnnotations;

namespace FlashCard.Api.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    public ICollection<Generation> Generations { get; set; } = new List<Generation>();
    public ICollection<GenerationErrorLog> GenerationErrorLogs { get; set; } = new List<GenerationErrorLog>();
} 