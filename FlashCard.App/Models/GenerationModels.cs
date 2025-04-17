using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FlashCard.App.Models;

public class GenerationRequest
{
    [Required]
    [StringLength(10000, MinimumLength = 1000, ErrorMessage = "Tekst źródłowy musi mieć od 1000 do 10000 znaków")]
    public string SourceText { get; set; } = string.Empty;
    
    [Required]
    public string Model { get; set; } = string.Empty;
}

public class GenerationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Model { get; set; } = string.Empty;
    public int GeneratedCount { get; set; }
    public List<GeneratedFlashcard> Flashcards { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class GeneratedFlashcard
{
    [Required]
    public object Front { get; set; } = new();
    
    [Required]
    public object Back { get; set; } = new();

    public int? GenerationId { get; set; }
    
    // Pomocnicze właściwości do wyświetlania w UI
    [JsonIgnore]
    public string FrontDisplay
    {
        get
        {
            //return $"{Front.Translation} - {Front.Word}\n{Front.Definition}\n{Front.Example}";
            try
            {
                // Obsługa string
                if (Front is string frontStr)
                {
                    return frontStr;
                }
                
                // Obsługa JsonElement
                if (Front is JsonElement jsonElement)
                {
                    // Próba pobrania właściwości Word
                    if (jsonElement.TryGetProperty("Word", out var word))
                    {
                        string wordValue = word.GetString() ?? "";
                        
                        // Sprawdź czy mamy też definicję
                        if (jsonElement.TryGetProperty("Definition", out var definition))
                        {
                            string definitionValue = definition.GetString() ?? "";
                            return wordValue + (string.IsNullOrEmpty(definitionValue) ? "" : " - " + definitionValue);
                        }
                        
                        return wordValue;
                    }
                    
                    // Jeśli nie ma Word, po prostu zwróć jako string
                    return jsonElement.ToString();
                }
                
                return Front?.ToString() ?? string.Empty;
            }
            catch
            {
                return Front?.ToString() ?? string.Empty;
            }
        }
    }
    
    [JsonIgnore]
    public string BackDisplay
    {
        get
        {
            try
            {
                // Obsługa string
                if (Back is string backStr)
                {
                    return backStr;
                }
                
                // Obsługa JsonElement
                if (Back is JsonElement jsonElement)
                {
                    // Próba pobrania właściwości Example_Translation
                    if (jsonElement.TryGetProperty("Example_Translation", out var translation))
                    {
                        return translation.GetString() ?? jsonElement.ToString();
                    }
                    
                    // Jeśli nie ma Example_Translation, po prostu zwróć jako string
                    return jsonElement.ToString();
                }
                
                return Back?.ToString() ?? string.Empty;
            }
            catch
            {
                return Back?.ToString() ?? string.Empty;
            }
        }
    }
}

public class BatchSaveRequest
{
    public List<GeneratedFlashcard> Flashcards { get; set; } = new();
} 