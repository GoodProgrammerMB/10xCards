using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Api.Models;

public class GenerationErrorLog
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string ErrorCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    public string SourceTextHash { get; set; } = string.Empty;
    
    [Required]
    public string ErrorMessage { get; set; } = string.Empty;
    
    [Required]
    public string ErrorDetails { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
} 