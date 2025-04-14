using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace FlashCard.E2ETests.Fixtures;

/// <summary>
/// Klasa pomocnicza do inicjalizacji i zasobów Playwright, współdzielonych między testami.
/// </summary>
public class PlaywrightFixture : IAsyncDisposable
{
    public IPlaywright Playwright { get; private set; }
    public IBrowser Browser { get; private set; }
    
    public string BaseUrl { get; }
    
    public PlaywrightFixture(string baseUrl = "https://localhost:5001")
    {
        BaseUrl = baseUrl;
    }
    
    /// <summary>
    /// Inicjalizuje zasoby Playwright.
    /// </summary>
    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            SlowMo = 100
        });
    }
    
    /// <summary>
    /// Tworzy nową stronę w nowym kontekście przeglądarki.
    /// </summary>
    public async Task<(IBrowserContext context, IPage page)> CreatePageAsync()
    {
        var context = await Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        return (context, page);
    }
    
    /// <summary>
    /// Zwalnia zasoby Playwright.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Browser != null)
            await Browser.DisposeAsync();
            
        Playwright?.Dispose();
    }
    
    /// <summary>
    /// Instaluje wymagane przeglądarki dla Playwright.
    /// Ta metoda powinna być wywołana przed uruchomieniem testów.
    /// </summary>
    public static async Task InstallBrowsersAsync()
    {
        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "--with-deps" });
        if (exitCode != 0)
            throw new Exception($"Playwright browsers installation failed with exit code {exitCode}");
    }
} 