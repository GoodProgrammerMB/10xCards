using System.Net;
using System.Linq.Expressions;
using FlashCard.Api.Configuration;
using FlashCard.Api.Data;
using FlashCard.Api.Models;
using FlashCard.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace FlashCard.Api.Tests;

public class GenerationServiceTests
{
    private readonly Mock<ILogger<GenerationService>> _loggerMock;
    private readonly Mock<IOptions<OpenRouterOptions>> _optionsMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly MockRepository _mockRepository;
    private readonly OpenRouterOptions _options;
    
    public GenerationServiceTests()
    {
        _mockRepository = new MockRepository(MockBehavior.Default);
        _loggerMock = _mockRepository.Create<ILogger<GenerationService>>();
        _httpMessageHandlerMock = _mockRepository.Create<HttpMessageHandler>();
        
        _options = new OpenRouterOptions
        {
            ApiKey = "test-key",
            BaseUrl = "https://api.test.com",
            DefaultModel = "test-model",
            TimeoutSeconds = 30
        };
        
        _optionsMock = _mockRepository.Create<IOptions<OpenRouterOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(_options);
    }
    
    private FlashCardDbContext CreateMockDbContext()
    {
        // Używamy prawdziwego kontekstu bazy danych w pamięci zamiast mocka
        var options = new DbContextOptionsBuilder<FlashCardDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        var dbContext = new FlashCardDbContext(options);
        
        // Upewniamy się, że baza danych jest pusta
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        
        return dbContext;
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
        
        // Symuluje odpowiedź API z uwzględnieniem nowego formatu JSON
        const string jsonResponse = @"{""choices"":[{""message"":{""content"":""[{""front"":""Test Question"",""back"":""Test Answer""},{""front"":""Second Question"",""back"":""Second Answer""}]""}}]}";
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
        var dbContext = CreateMockDbContext();
        
        var service = new GenerationService(_loggerMock.Object, httpClient, dbContext, _optionsMock.Object);
        
        // Act
        var result = await service.GenerateFlashcardsAsync(request, userId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(2, result.Flashcards.Count);
        Assert.Equal("Test Question", result.Flashcards[0].Front.ToString());
        Assert.Equal("Test Answer", result.Flashcards[0].Back.ToString());
        Assert.Equal("Second Question", result.Flashcards[1].Front.ToString());
        Assert.Equal("Second Answer", result.Flashcards[1].Back.ToString());
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
        var dbContext = CreateMockDbContext();
        
        var service = new GenerationService(_loggerMock.Object, httpClient, dbContext, _optionsMock.Object);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => 
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