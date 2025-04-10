using System.ComponentModel.DataAnnotations;

namespace FlashCard.Api.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Flashcard> Flashcards { get; set; }
    public virtual ICollection<Generation> Generations { get; set; }
    public virtual ICollection<GenerationErrorLog> GenerationErrorLogs { get; set; }

    public User()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Flashcards = new HashSet<Flashcard>();
        Generations = new HashSet<Generation>();
        GenerationErrorLogs = new HashSet<GenerationErrorLog>();
    }
} 