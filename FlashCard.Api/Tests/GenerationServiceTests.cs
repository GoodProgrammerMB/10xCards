using System.Net;
using FlashCard.Api.Configuration;
using FlashCard.Api.Data;
using FlashCard.Api.Models;
using FlashCard.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace FlashCard.Api.Tests;

public class GenerationServiceTests
{
    private readonly Mock<ILogger<GenerationService>> _loggerMock;
    private readonly Mock<FlashCardDbContext> _dbContextMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly OpenRouterOptions _options;
    
    public GenerationServiceTests()
    {
        _loggerMock = new Mock<ILogger<GenerationService>>();
        _dbContextMock = new Mock<FlashCardDbContext>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _options = new OpenRouterOptions
        {
            ApiKey = "test-key",
            BaseUrl = "https://api.test.com",
            DefaultModel = "test-model",
            TimeoutSeconds = 30
        };
    }
    
    [Fact]
    public async Task GenerateFlashcardsAsync_SuccessfulGeneration_ReturnsFlashcards()
    {
        // Arrange
        var userId = 1;
        var request = new GenerationRequestDto
        {
            SourceText = new string('a', 1000),
            Model = "test-model"
        };
        
        const string jsonResponse = @"{""choices"":[{""message"":{""content"":""[{\\""front\\"":""Test Question"",\\""back\\"":""Test Answer"",\\""source\\"":""ai-full""}]""}}]}";
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
        var optionsMock = new Mock<IOptions<OpenRouterOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        
        var service = new GenerationService(_loggerMock.Object, httpClient, _dbContextMock.Object, optionsMock.Object);
        
        // Act
        var result = await service.GenerateFlashcardsAsync(request, userId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Single(result.Flashcards);
        Assert.Equal("Test Question", result.Flashcards[0].Front);
        Assert.Equal("Test Answer", result.Flashcards[0].Back);
        Assert.Equal("ai-full", result.Flashcards[0].Source);
    }
    
    [Fact]
    public async Task GenerateFlashcardsAsync_ApiError_LogsErrorAndThrows()
    {
        // Arrange
        var userId = 1;
        var request = new GenerationRequestDto
        {
            SourceText = new string('a', 1000),
            Model = "test-model"
        };
        
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
        var optionsMock = new Mock<IOptions<OpenRouterOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        
        var service = new GenerationService(_loggerMock.Object, httpClient, _dbContextMock.Object, optionsMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            service.GenerateFlashcardsAsync(request, userId));
            
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
} 