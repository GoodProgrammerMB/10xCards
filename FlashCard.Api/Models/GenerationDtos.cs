using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlashCard.Api.Models;

public class GenerationRequestDto
{
    [Required]
    [StringLength(10000, MinimumLength = 1000)]
    public string SourceText { get; set; } = string.Empty;
    
    public string? Model { get; set; }
}

public class GenerationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Model { get; set; } = string.Empty;
    public int GeneratedCount { get; set; }
    public List<GenerationFlashcardDto> Flashcards { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class FrontData
{
    public string Word { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
}

public class BackData
{
    public string Example_Translation { get; set; } = string.Empty;
}

public class GenerationFlashcardDto
{
    [JsonPropertyName("front")]
    [JsonPropertyOrder(1)]
    public FrontData Front { get; set; } = new FrontData();

    [JsonPropertyName("back")]
    [JsonPropertyOrder(2)]
    public BackData Back { get; set; } = new BackData();

    // Możesz dodać właściwości pomocnicze, które konwertują obiekty na stringi
    [JsonIgnore]
    public string FrontAsString
    {
        get
        {
            return $"{Front.Translation} - {Front.Word}\n{Front.Definition}\n{Front.Example}";
        }
    }

    [JsonIgnore]
    public string BackAsString => Back.Example_Translation;

    // Twoje istniejące właściwości pomocnicze można dostosować
    [JsonIgnore]
    public string? question { get => FrontAsString; set { } }

    [JsonIgnore]
    public string? answer { get => BackAsString; set { } }

    [JsonIgnore]
    public string? term { get => FrontAsString; set { } }

    [JsonIgnore]
    public string? definition { get => BackAsString; set { } }
} 