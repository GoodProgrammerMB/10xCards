using Microsoft.Playwright;

namespace FlashCard.E2ETests.PageObjects;

/// <summary>
/// Reprezentuje stronę logowania w aplikacji.
/// </summary>
public class LoginPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;
    
    // Selektory dla elementów na stronie
    private readonly string _emailInputSelector = "#email";
    private readonly string _passwordInputSelector = "#password";
    private readonly string _loginButtonSelector = "button[type='submit']";
    private readonly string _errorMessageSelector = ".mud-alert-message";
    
    public LoginPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }
    
    /// <summary>
    /// Przechodzi do strony logowania.
    /// </summary>
    public async Task GoToAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/login");
        await _page.WaitForSelectorAsync(_emailInputSelector);
    }
    
    /// <summary>
    /// Loguje użytkownika do aplikacji.
    /// </summary>
    /// <param name="email">Adres email użytkownika</param>
    /// <param name="password">Hasło użytkownika</param>
    /// <returns>True jeśli logowanie się powiodło, false w przeciwnym wypadku</returns>
    public async Task<bool> LoginAsync(string email, string password)
    {
        await _page.FillAsync(_emailInputSelector, email);
        await _page.FillAsync(_passwordInputSelector, password);
        
        await _page.ClickAsync(_loginButtonSelector);
        
        try 
        {
            // Sprawdź, czy użytkownik został przekierowany na stronę główną
            await _page.WaitForURLAsync($"{_baseUrl}/", new() { Timeout = 5000 });
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Pobiera komunikat o błędzie, jeśli logowanie się nie powiodło.
    /// </summary>
    /// <returns>Komunikat o błędzie lub null, jeśli nie znaleziono komunikatu</returns>
    public async Task<string?> GetErrorMessageAsync()
    {
        try
        {
            var errorMessage = await _page.TextContentAsync(_errorMessageSelector);
            return errorMessage;
        }
        catch
        {
            return null;
        }
    }
} 