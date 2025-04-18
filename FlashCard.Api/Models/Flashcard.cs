using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Api.Models;

public class Flashcard
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Front { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Back { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Source { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign keys
    public int UserId { get; set; }
    public int? GenerationId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    [ForeignKey(nameof(GenerationId))]
    public virtual Generation? Generation { get; set; }
    
    public Flashcard()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Source = "manual";
    }
} 