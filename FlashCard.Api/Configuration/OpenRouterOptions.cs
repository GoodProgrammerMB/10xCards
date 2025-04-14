namespace FlashCard.Api.Configuration;

public class OpenRouterOptions
{
    public const string SectionName = "OpenRouter";
    
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
    public string ApiEndpoint { get; set; } = "/chat/completions";
    public string DefaultModel { get; set; } = "anthropic/claude-3-sonnet";
    public int TimeoutSeconds { get; set; } = 120;
    public string SiteUrl { get; set; } = "https://github.com/10xCards/FlashCard";
    public string SiteName { get; set; } = "FlashCard";
} 