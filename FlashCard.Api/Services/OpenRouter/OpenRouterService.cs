using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FlashCard.Api.Services.OpenRouter.Models;
using FlashCard.Api.Services.OpenRouter.Exceptions;

namespace FlashCard.Api.Services.OpenRouter;

public class OpenRouterService : IOpenRouterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenRouterService> _logger;
    private readonly string _apiKey;
    private readonly string _apiEndpoint;
    private readonly Dictionary<string, object> _defaultParameters;

    public string ApiEndpoint => _apiEndpoint;
    public string DefaultModelName { get; private set; }
    public IReadOnlyDictionary<string, object> DefaultParameters => _defaultParameters;

    public OpenRouterService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenRouterService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _apiKey = configuration["OpenRouter:ApiKey"]
            ?? throw new ArgumentException("OpenRouter API key not found in configuration");
        _apiEndpoint = configuration["OpenRouter:ApiEndpoint"]
            ?? throw new ArgumentException("OpenRouter API endpoint not found in configuration");

        DefaultModelName = configuration["OpenRouter:DefaultModel"] ?? "openrouter-llm-v1";

        _defaultParameters = new Dictionary<string, object>
        {
            { "temperature", 0.7 },
            { "top_p", 0.95 },
            { "max_tokens", 150 }
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    private OpenRouterRequest BuildPayload(
        string userMessage,
        string? systemMessage,
        string? modelName,
        Dictionary<string, object>? parameters,
        ResponseFormat? responseFormat)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            throw new OpenRouterValidationException("User message cannot be empty");
        }

        var messages = new List<Message>();

        if (!string.IsNullOrWhiteSpace(systemMessage))
        {
            messages.Add(new Message
            {
                Role = "system",
                Content = systemMessage
            });
        }

        messages.Add(new Message
        {
            Role = "user",
            Content = userMessage
        });

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
            ResponseFormat = responseFormat,
            Temperature = Convert.ToDouble(mergedParameters["temperature"]),
            TopP = Convert.ToDouble(mergedParameters["top_p"]),
            MaxTokens = Convert.ToInt32(mergedParameters["max_tokens"])
        };
    }

    private void ParseResponse(OpenRouterResponse response)
    {
        if (response.Choices == null || response.Choices.Count == 0)
        {
            throw new OpenRouterValidationException("Response contains no choices");
        }

        var choice = response.Choices[0];
        if (choice.Message == null || string.IsNullOrWhiteSpace(choice.Message.Content))
        {
            throw new OpenRouterValidationException("Response message is empty");
        }

        if (response.Usage == null)
        {
            _logger.LogWarning("Response usage information is missing");
        }
        else
        {
            _logger.LogInformation(
                "OpenRouter API usage - Prompt tokens: {PromptTokens}, Completion tokens: {CompletionTokens}, Total tokens: {TotalTokens}",
                response.Usage.PromptTokens,
                response.Usage.CompletionTokens,
                response.Usage.TotalTokens);
        }

        if (choice.FinishReason != "stop" && choice.FinishReason != "length")
        {
            _logger.LogWarning("Unexpected finish reason: {FinishReason}", choice.FinishReason);
        }
    }

    public async Task<string> GetResponseContent(OpenRouterResponse response)
    {
        ParseResponse(response);
        return response.Choices[0].Message.Content;
    }

    public async Task<OpenRouterResponse> SendRequest(
        string userMessage,
        string? systemMessage = null,
        string? modelName = null,
        Dictionary<string, object>? parameters = null,
        ResponseFormat? responseFormat = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = BuildPayload(userMessage, systemMessage, modelName, parameters, responseFormat);

            using var response = await _httpClient.PostAsJsonAsync(_apiEndpoint, request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OpenRouter API error: {StatusCode} - {Error}", response.StatusCode, errorContent);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new OpenRouterAuthenticationException("Invalid API key or authentication failed");
                }

                throw new OpenRouterCommunicationException(
                    $"API request failed with status code {response.StatusCode}: {errorContent}",
                    (int)response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<OpenRouterResponse>(
                cancellationToken: cancellationToken);

            if (result == null)
            {
                throw new OpenRouterValidationException("Received null response from API");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize OpenRouter API response");
            throw new OpenRouterValidationException("Failed to parse API response", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to OpenRouter API failed");
            throw new OpenRouterCommunicationException("Failed to communicate with API", 503);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request to OpenRouter API was cancelled");
            throw;
        }
        catch (Exception ex) when (ex is not OpenRouterException)
        {
            _logger.LogError(ex, "Unexpected error during OpenRouter API request");
            throw new OpenRouterException("An unexpected error occurred", ex);
        }
    }
} 