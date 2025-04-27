using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Api.Models;

public class Flashcard
{
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Front { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
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
    
    // Learning related properties
    public DateTime? LastReviewedAt { get; set; }
    public DateTime? NextReviewAt { get; set; }
    public int CorrectAnswersInRow { get; set; } = 0;
    public bool IsLearned { get; set; } = false;
    
    public Flashcard()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Source = "manual";
    }
} 