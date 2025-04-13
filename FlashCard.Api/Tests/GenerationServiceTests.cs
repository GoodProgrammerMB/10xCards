using FlashCard.Api.Data;
using FlashCard.Api.Models;
using FlashCard.Api.Services;
using FlashCard.Api.Services.OpenRouter;
using FlashCard.Api.Services.OpenRouter.Models;
using Moq;
using Xunit;

namespace FlashCard.Api.Tests;

public class GenerationServiceTests
{
    private readonly Mock<ILogger<GenerationService>> _loggerMock;
    private readonly Mock<FlashCardDbContext> _dbContextMock;
    private readonly Mock<IOpenRouterService> _openRouterServiceMock;
    
    public GenerationServiceTests()
    {
        _loggerMock = new Mock<ILogger<GenerationService>>();
        _dbContextMock = new Mock<FlashCardDbContext>();
        _openRouterServiceMock = new Mock<IOpenRouterService>();
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
        
        var jsonResponse = @"[{""front"":""Test Question"",""back"":""Test Answer"",""source"":""ai-full""}]";
        var openRouterResponse = new OpenRouterResponse
        {
            Choices = new List<OpenRouterChoice>
            {
                new OpenRouterChoice
                {
                    Message = new OpenRouterMessage
                    {
                        Content = jsonResponse
                    }
                }
            }
        };
        
        _openRouterServiceMock
            .Setup(x => x.DefaultModelName)
            .Returns("test-model");
            
        _openRouterServiceMock
            .Setup(x => x.SendRequest(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<ResponseFormat>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(openRouterResponse);
            
        _openRouterServiceMock
            .Setup(x => x.GetResponseContent(It.IsAny<OpenRouterResponse>()))
            .ReturnsAsync(jsonResponse);
        
        var service = new GenerationService(_loggerMock.Object, _dbContextMock.Object, _openRouterServiceMock.Object);
        
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
        
        _openRouterServiceMock
            .Setup(x => x.DefaultModelName)
            .Returns("test-model");
            
        _openRouterServiceMock
            .Setup(x => x.SendRequest(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<ResponseFormat>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("API Error"));
        
        var service = new GenerationService(_loggerMock.Object, _dbContextMock.Object, _openRouterServiceMock.Object);
        
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