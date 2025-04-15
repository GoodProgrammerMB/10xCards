using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FlashCard.Api.Services.OpenRouter.Models;
using FlashCard.Api.Application.Exceptions;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

namespace FlashCard.Api.Services;

public interface IOpenApiService
{
    string ApiEndpoint { get; }
    string DefaultModelName { get; }
    IReadOnlyDictionary<string, object> DefaultParameters { get; }
    
    Task<string> GetChatResponseAsync(
        string userMessage,
        string? systemMessage = null,
        string? modelName = null,
        Dictionary<string, object>? parameters = null,
        ResponseFormat? responseFormat = null,
        CancellationToken cancellationToken = default);
}

public class OpenApiService : IOpenApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenApiService> _logger;
    private readonly Dictionary<string, object> _defaultParameters;

    public string ApiEndpoint { get; }
    public string DefaultModelName { get; }
    public IReadOnlyDictionary<string, object> DefaultParameters => _defaultParameters;

    public OpenApiService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenApiService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _logger.LogInformation("Initializing OpenApiService...");
        
        var baseUrl = configuration["OpenRouter:BaseUrl"] 
            ?? throw new ArgumentException("OpenAI base URL not found in configuration");
        
        // Tworzenie pełnego URL API
        var apiPath = configuration["OpenRouter:ApiEndpoint"] ?? "/api/v1/chat/completions";
        ApiEndpoint = baseUrl.TrimEnd('/') + (apiPath.StartsWith("/") ? apiPath : "/" + apiPath);
        
        var apiKey = configuration["OpenRouter:ApiKey"] 
            ?? throw new ArgumentException("OpenAI API key not found in configuration");
        
        // Usuń prefix "Bearer " jeśli istnieje
        apiKey = apiKey.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? apiKey.Substring("Bearer ".Length).Trim()
            : apiKey.Trim();
        
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length < 20)
        {
            throw new OpenRouterAuthenticationException("API key seems invalid (too short or empty)");
        }
        
        DefaultModelName = configuration["OpenRouter:DefaultModel"] ?? "gpt-4";
        
        _defaultParameters = new Dictionary<string, object>
        {
            { "temperature", 0.7 },
            { "top_p", 0.95 },
            { "max_tokens", 150 }
        };

        // Konfiguracja HttpClient
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var siteUrl = configuration["OpenRouter:SiteUrl"] ?? "https://github.com/10xCards/FlashCard";
        var siteName = configuration["OpenRouter:SiteName"] ?? "FlashCard";
        
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", siteUrl);
        _httpClient.DefaultRequestHeaders.Add("X-Title", siteName);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("FlashCard/1.0");
        
        _logger.LogInformation("OpenApiService initialized successfully");
    }

    private OpenRouterRequest BuildRequest(
        string userMessage,
        string? systemMessage,
        string? modelName,
        Dictionary<string, object>? parameters)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            throw new OpenRouterValidationException("User message cannot be empty");
        }

        var messages = new List<Message>();
        
        if (!string.IsNullOrWhiteSpace(systemMessage))
        {
            messages.Add(new Message { Role = "system", Content = systemMessage });
        }

        messages.Add(new Message { Role = "user", Content = userMessage });

        var mergedParameters = new Dictionary<string, object>(_defaultParameters);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                mergedParameters[param.Key] = param.Value;
            }
        }

        return new OpenRouterRequest
        {
            Messages = messages,
            Model = modelName ?? DefaultModelName,
            Temperature = Convert.ToDouble(mergedParameters["temperature"]),
            TopP = Convert.ToDouble(mergedParameters["top_p"]),
            MaxTokens = Convert.ToInt32(mergedParameters["max_tokens"])
        };
    }

    public async Task<string> GetChatResponseAsync(
        string userMessage,
        string? systemMessage = null,
        string? modelName = null,
        Dictionary<string, object>? parameters = null,
        ResponseFormat? responseFormat = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = BuildRequest(userMessage, systemMessage, modelName, parameters);
            
            if (responseFormat != null)
            {
                request = request with { ResponseFormat = responseFormat };
            }
            
            var jsonOptions = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            var requestJson = JsonSerializer.Serialize(request, jsonOptions);
            _logger.LogDebug("Sending request to OpenAI. Payload: {Payload}", requestJson);
            
            using var content = JsonContent.Create(request, null, jsonOptions);
            using var response = await _httpClient.PostAsync(ApiEndpoint, content, cancellationToken);
            
            var rawContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI API error: {StatusCode} - {Error}", response.StatusCode, rawContent);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new OpenRouterAuthenticationException("Invalid API key or authentication failed");
                }
                
                throw new OpenRouterCommunicationException(
                    $"API request failed with status code {response.StatusCode}: {rawContent}",
                    (int)response.StatusCode);
            }

            try 
            {
                var result = JsonSerializer.Deserialize<OpenRouterResponse>(rawContent, jsonOptions);

                if (result == null)
                {
                    throw new OpenRouterValidationException("Received null response from API");
                }
                
                if (result.Choices == null || result.Choices.Count == 0 || 
                    result.Choices[0].Message == null || string.IsNullOrWhiteSpace(result.Choices[0].Message.Content))
                {
                    throw new OpenRouterValidationException("Response contains no valid content");
                }
                
                if (result.Usage != null)
                {
                    _logger.LogInformation(
                        "API usage - Prompt tokens: {PromptTokens}, Completion tokens: {CompletionTokens}, Total: {TotalTokens}",
                        result.Usage.PromptTokens,
                        result.Usage.CompletionTokens,
                        result.Usage.TotalTokens);
                }

                return result.Choices[0].Message.Content;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse response JSON. Raw content: {Content}", rawContent);
                throw new OpenRouterValidationException($"Failed to parse API response: {ex.Message}", ex);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during API request");
            throw new OpenRouterCommunicationException($"HTTP error: {ex.Message}", 0);
        }
        catch (OpenRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during API request");
            throw new OpenRouterException("Unexpected error during API request", ex);
        }
    }
} 