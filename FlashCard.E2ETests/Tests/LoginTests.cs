using FlashCard.E2ETests.PageObjects;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlashCard.E2ETests.Tests;

[TestClass]
public class LoginTests
{
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private LoginPage _loginPage;
    
    private const string BaseUrl = "https://localhost:5001";
    
    [TestInitialize]
    public async Task TestInitialize()
    {
        // Inicjalizacja Playwright
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
        
        _loginPage = new LoginPage(_page, BaseUrl);
    }
    
    [TestCleanup]
    public async Task TestCleanup()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
    }
    
    [TestMethod]
    public async Task Login_WithValidCredentials_ShouldLoginSuccessfully()
    {
        // Arrange
        await _loginPage.GoToAsync();
        
        // Act
        var result = await _loginPage.LoginAsync("test@example.com", "Password123!");
        
        // Assert
        Assert.IsTrue(result, "Login powinien zakończyć się powodzeniem");
    }
    
    [TestMethod]
    public async Task Login_WithInvalidCredentials_ShouldShowErrorMessage()
    {
        // Arrange
        await _loginPage.GoToAsync();
        
        // Act
        var result = await _loginPage.LoginAsync("invalid@example.com", "WrongPassword!");
        var errorMessage = await _loginPage.GetErrorMessageAsync();
        
        // Assert
        Assert.IsFalse(result, "Login nie powinien zakończyć się powodzeniem");
        Assert.IsNotNull(errorMessage, "Komunikat o błędzie powinien zostać wyświetlony");
    }
} 