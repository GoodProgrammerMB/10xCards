using FlashCard.Api.Services.OpenRouter.Models;

namespace FlashCard.Api.Services.OpenRouter
{
    public interface IOpenRouterService
    {
        string ApiEndpoint { get; }
        string DefaultModelName { get; }
        IReadOnlyDictionary<string, object> DefaultParameters { get; }

        Task<string> GetResponseContent(OpenRouterResponse response);
        Task<OpenRouterResponse> SendRequest(string userMessage, string? systemMessage = null, string? modelName = null, Dictionary<string, object>? parameters = null, ResponseFormat? responseFormat = null, CancellationToken cancellationToken = default);
    }
}