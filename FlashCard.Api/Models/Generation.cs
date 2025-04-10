using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Api.Models;

public class Generation
{
    [Key]
    public int Id { get; set; }
    
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
    public User? User { get; set; }
    public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
} 