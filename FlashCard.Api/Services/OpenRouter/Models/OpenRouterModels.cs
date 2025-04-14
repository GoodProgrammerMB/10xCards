using System.Text.Json.Serialization;

namespace FlashCard.Api.Services.OpenRouter.Models;

public record OpenRouterRequest
{
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; init; } = new();

    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;

    [JsonPropertyName("response_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
    public string Type { get; init; } = "text";

    [JsonPropertyName("schema")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Schema { get; init; }
}

public record OpenRouterResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; init; } = new();

    [JsonPropertyName("created")]
    public long Created { get; init; }

    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;

    [JsonPropertyName("object")]
    public string Object { get; init; } = string.Empty;

    [JsonPropertyName("usage")]
    public Usage? Usage { get; init; }
}

public record Choice
{
    [JsonPropertyName("message")]
    public Message Message { get; init; } = new();

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; init; } = string.Empty;

    [JsonPropertyName("index")]
    public int Index { get; init; }
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