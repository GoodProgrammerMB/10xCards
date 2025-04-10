namespace FlashCard.Api.Configuration;

public class OpenRouterOptions
{
    public const string SectionName = "OpenRouter";
    
    public string ApiKey { get; set; } = "eee-ee-ee";
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
    public string DefaultModel { get; set; } = "anthropic/claude-3-sonnet";
    public int TimeoutSeconds { get; set; } = 120;
} 