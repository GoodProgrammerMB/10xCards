using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Api.Models;

public class Generation
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    public string SourceTextHash { get; set; } = string.Empty;
    
    [Required]
    public int GeneratedCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Flashcard> Flashcards { get; set; } = new HashSet<Flashcard>();
    public virtual ICollection<GenerationErrorLog> GenerationErrorLogs { get; set; } = new HashSet<GenerationErrorLog>();

    public Generation()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Flashcards = new HashSet<Flashcard>();
        GenerationErrorLogs = new HashSet<GenerationErrorLog>();
    }
} 