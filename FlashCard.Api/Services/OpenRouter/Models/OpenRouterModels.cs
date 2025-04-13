using System.Text.Json.Serialization;

namespace FlashCard.Api.Services.OpenRouter.Models;

public record OpenRouterRequest
{
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; init; } = new();

    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;

    [JsonPropertyName("response_format")]
    public ResponseFormat? ResponseFormat { get; init; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; init; }

    [JsonPropertyName("top_p")]
    public double TopP { get; init; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; init; }
}

public record Message
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;
}

public record ResponseFormat
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "json_object";
}

public class OpenRouterResponse
{
    public List<OpenRouterChoice>? Choices { get; set; }
    public Usage? Usage { get; set; }
}

public class OpenRouterChoice
{
    public OpenRouterMessage? Message { get; set; }
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

public class OpenRouterMessage
{
    public string? Content { get; set; }
}

public record Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
} 