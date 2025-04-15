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
    
    private const string BaseUrl = "http://localhost:5007";
    
    [TestInitialize]
    public async Task TestInitialize()
    {
        // Inicjalizacja Playwright
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false // Ustawiamy na false, aby widzieć przeglądarkę
        });
        
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true // Ignoruj błędy HTTPS/SSL
        });
        
        _page = await _context.NewPageAsync();
        
        // Dodajemy obsługę konsoli
        _page.Console += (_, msg) => 
        {
            System.Console.WriteLine($"Konsola: [{msg.Type}] {msg.Text}");
        };
        
        _loginPage = new LoginPage(_page, BaseUrl);
    }
    
    [TestCleanup]
    public async Task TestCleanup()
    {
        // Wykonanie zrzutu ekranu przed zamknięciem
        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "login-test-cleanup.png" });
        
        await _context.CloseAsync();
        await _browser.CloseAsync();
    }
    
    [TestMethod]
    public async Task Login_WithValidCredentials_ShouldLoginSuccessfully()
    {
        // Arrange
        await _loginPage.GoToAsync();
        
        // Wykonanie zrzutu ekranu przed logowaniem
        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "before-login.png" });
        
        // Act
        var result = await _loginPage.LoginAsync("marcin@wp.pl", "marcin");
        
        // Wykonanie zrzutu ekranu po logowaniu
        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "after-login.png" });
        
        // Assert
        Assert.IsTrue(result, "Logowanie powinno zakończyć się powodzeniem");
        
        // Sprawdzamy, czy na stronie istnieją elementy menu
        bool hasMainMenu = await CheckMainMenuElements();
        Assert.IsTrue(hasMainMenu, "Na stronie powinny być widoczne elementy menu po zalogowaniu");
    }
    
    /// <summary>
    /// Sprawdza, czy na stronie istnieją elementy menu po zalogowaniu
    /// </summary>
    private async Task<bool> CheckMainMenuElements()
    {
        try
        {
            // Szukamy elementów menu, które powinny być widoczne po zalogowaniu
            var menuItems = await _page.QuerySelectorAllAsync(".mud-nav-link");
            System.Console.WriteLine($"Znaleziono {menuItems.Count} elementów menu");
            
            // Wypisujemy znalezione elementy menu dla diagnostyki
            foreach (var item in menuItems)
            {
                var text = await item.TextContentAsync();
                System.Console.WriteLine($"Element menu: {text}");
            }
            
            // Jeśli znaleziono jakiekolwiek elementy menu, uznajemy że test zakończył się sukcesem
            return menuItems.Count > 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Błąd podczas sprawdzania elementów menu: {ex.Message}");
            return false;
        }
    }
    
    [TestMethod]
    public async Task Login_WithInvalidCredentials_ShouldShowErrorMessage()
    {
        // Arrange
        await _loginPage.GoToAsync();
        
        // Wykonanie zrzutu ekranu przed logowaniem
        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "before-invalid-login.png" });
        
        // Act - używamy poprawnego emaila, ale z niepoprawnym hasłem (dodajemy cyfry)
        // Nie sprawdzamy wartości zwracanej przez metodę LoginAsync, bo może być błędna
        await _loginPage.LoginAsync("marcin@wp.pl", "marcin123");
        
        // Wykonanie zrzutu ekranu po próbie logowania
        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "after-invalid-login.png" });
        
        // Sprawdzamy bezpośrednio, czy jest komunikat błędu
        var errorElement = await _page.QuerySelectorAsync(".mud-alert");
        
        // Assert
        Assert.IsNotNull(errorElement, "Komunikat o błędzie powinien zostać wyświetlony");
        
        if (errorElement != null)
        {
            var errorText = await errorElement.TextContentAsync();
            System.Console.WriteLine($"Znaleziono komunikat o błędzie: {errorText}");
            
            // Sprawdzamy, czy komunikat zawiera informację o nieprawidłowym loginie lub haśle
            Assert.IsTrue(
                errorText.Contains("nieprawidłowy") || 
                errorText.Contains("Nieprawidłowy") || 
                errorText.Contains("błędne") ||
                errorText.Contains("Błędne"),
                "Komunikat o błędzie powinien informować o nieprawidłowym loginie lub haśle");
        }
    }
} 