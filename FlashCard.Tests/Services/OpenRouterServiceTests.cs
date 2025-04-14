using System.Net;
using FlashCard.Api.Application.Exceptions;
using FlashCard.Api.Services;
using FlashCard.Api.Services.OpenRouter.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace FlashCard.Tests.Services;

public class OpenRouterServiceTests
{
    private readonly Mock<ILogger<OpenApiService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    
    public OpenRouterServiceTests()
    {
        _loggerMock = new Mock<ILogger<OpenApiService>>();
        _configurationMock = new Mock<IConfiguration>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        
        // Konfiguracja mock dla IConfiguration
        _configurationMock.Setup(c => c["OpenRouter:BaseUrl"]).Returns("https://api.test.com");
        _configurationMock.Setup(c => c["OpenRouter:ApiEndpoint"]).Returns("/api/v1/chat/completions");
        _configurationMock.Setup(c => c["OpenRouter:ApiKey"]).Returns("sk-or-test-key-valid-format");
        _configurationMock.Setup(c => c["OpenRouter:DefaultModel"]).Returns("anthropic/claude-3-sonnet");
        _configurationMock.Setup(c => c["OpenRouter:SiteUrl"]).Returns("https://test.com");
        _configurationMock.Setup(c => c["OpenRouter:SiteName"]).Returns("Test App");
    }
    
    [Fact]
    public async Task GetChatResponseAsync_SuccessfulResponse_ReturnsContent()
    {
        // Arrange
        const string jsonResponse = @"{""id"":""test-id"",""choices"":[{""message"":{""content"":""Odpowiedź testowa""},""finish_reason"":""stop"",""index"":0}],""created"":1682089638,""model"":""anthropic/claude-3-sonnet"",""object"":""chat.completion"",""usage"":{""prompt_tokens"":10,""completion_tokens"":20,""total_tokens"":30}}";
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        };
        
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);
            
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        
        var service = new OpenApiService(httpClient, _configurationMock.Object, _loggerMock.Object);
        
        var userMessage = "Test message";
        
        // Act
        var result = await service.GetChatResponseAsync(userMessage);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Odpowiedź testowa", result);
    }
    
    [Fact]
    public async Task GetChatResponseAsync_ApiError_LogsErrorAndThrows()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("API Error")
        };
        
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);
            
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        
        var service = new OpenApiService(httpClient, _configurationMock.Object, _loggerMock.Object);
        
        var userMessage = "Test message";
        
        // Act & Assert
        await Assert.ThrowsAsync<OpenRouterCommunicationException>(() => 
            service.GetChatResponseAsync(userMessage));
            
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task GetChatResponseAsync_InvalidResponseFormat_ThrowsValidationException()
    {
        // Arrange
        const string jsonResponse = @"{""id"":""test-id"",""choices"":[],""created"":1682089638,""model"":""anthropic/claude-3-sonnet"",""object"":""chat.completion""}";
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        };
        
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);
            
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        
        var service = new OpenApiService(httpClient, _configurationMock.Object, _loggerMock.Object);
        
        var userMessage = "Test message";
        
        // Act & Assert
        await Assert.ThrowsAsync<OpenRouterValidationException>(() => 
            service.GetChatResponseAsync(userMessage));
    }
    
    [Fact]
    public async Task GetChatResponseAsync_WithSystemMessage_SendsCorrectRequest()
    {
        // Arrange
        const string jsonResponse = @"{""id"":""test-id"",""choices"":[{""message"":{""content"":""Odpowiedź testowa""},""finish_reason"":""stop"",""index"":0}],""created"":1682089638,""model"":""anthropic/claude-3-sonnet"",""object"":""chat.completion"",""usage"":{""prompt_tokens"":10,""completion_tokens"":20,""total_tokens"":30}}";
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        };
        
        HttpRequestMessage capturedRequest = null;
        
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(httpResponse);
            
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        
        var service = new OpenApiService(httpClient, _configurationMock.Object, _loggerMock.Object);
        
        var userMessage = "Test message";
        var systemMessage = "System instruction";
        
        // Act
        var result = await service.GetChatResponseAsync(userMessage, systemMessage);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Odpowiedź testowa", result);
        
        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri.ToString().EndsWith("/api/v1/chat/completions")),
                ItExpr.IsAny<CancellationToken>());
    }
} 