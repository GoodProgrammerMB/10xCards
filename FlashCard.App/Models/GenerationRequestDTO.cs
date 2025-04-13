using System.ComponentModel.DataAnnotations;

namespace FlashCard.App.Models;

public class GenerationRequestDTO
{
    [Required(ErrorMessage = "Tekst źródłowy jest wymagany")]
    [StringLength(10000, MinimumLength = 1000, ErrorMessage = "Tekst źródłowy musi mieć od 1000 do 10000 znaków")]
    public string SourceText { get; set; } = string.Empty;
    
    public string? Model { get; set; }
} 