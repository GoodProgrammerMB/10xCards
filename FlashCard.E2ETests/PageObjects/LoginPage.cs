using Microsoft.Playwright;

namespace FlashCard.E2ETests.PageObjects;

/// <summary>
/// Reprezentuje stronę logowania w aplikacji.
/// </summary>
public class LoginPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;
    
    // Selektory dla elementów na stronie - uproszczone
    private readonly string _loginTabSelector = "button:has-text('Logowanie')";
    private readonly string _emailInputSelector = "input[type='email']";
    private readonly string _passwordInputSelector = "input[type='password']";
    private readonly string _loginButtonSelector = "button:has-text('Zaloguj się'), button:has-text('Zaloguj')";
    private readonly string _errorMessageSelector = ".mud-alert";
    
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
        Console.WriteLine("Przechodzę do strony logowania...");
        
        // Przechodzimy na stronę główną
        await _page.GotoAsync(_baseUrl);
        await Task.Delay(2000); // Krókie opóźnienie po załadowaniu strony
        
        try
        {
            // Czekamy na załadowanie strony
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            Console.WriteLine("Strona załadowana, szukam zakładki Logowanie...");
            
            // Sprawdzamy, czy jesteśmy już na zakładce logowania
            var emailInput = await _page.QuerySelectorAsync(_emailInputSelector);
            if (emailInput != null && await emailInput.IsVisibleAsync())
            {
                Console.WriteLine("Już jesteśmy na zakładce logowania.");
                return;
            }
            
            // Szukamy i klikamy zakładkę logowania
            var loginTab = await _page.QuerySelectorAsync(_loginTabSelector);
            if (loginTab != null && await loginTab.IsVisibleAsync())
            {
                Console.WriteLine("Klikam zakładkę logowania...");
                await loginTab.ClickAsync();
                await Task.Delay(1000);
            }
            
            // Czekamy na załadowanie formularza
            await Task.Delay(2000);
            
            // Zrzut ekranu dla diagnostyki
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "login-tab-clicked.png" });
            
            // Wypisujemy znalezione pola input
            var inputs = await _page.QuerySelectorAllAsync("input");
            Console.WriteLine($"Znaleziono {inputs.Count} pól input po przejściu do zakładki logowania:");
            foreach (var input in inputs)
            {
                var type = await input.GetAttributeAsync("type");
                var id = await input.GetAttributeAsync("id");
                var name = await input.GetAttributeAsync("name");
                var isVisible = await input.IsVisibleAsync();
                Console.WriteLine($"Input: type={type}, id={id}, name={name}, widoczny: {isVisible}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas przechodzenia do strony logowania: {ex.Message}");
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "login-tab-error.png" });
            throw;
        }
    }
    
    /// <summary>
    /// Loguje użytkownika do aplikacji.
    /// </summary>
    /// <param name="email">Adres email użytkownika</param>
    /// <param name="password">Hasło użytkownika</param>
    /// <returns>True jeśli logowanie się powiodło, false w przeciwnym wypadku</returns>
    public async Task<bool> LoginAsync(string email, string password)
    {
        Console.WriteLine($"Próbuję zalogować się jako: {email}");
        
        try
        {
            // Zrzut ekranu przed wypełnianiem formularza
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "before-login.png" });
            
            // Wypełniamy formularz - używając metody z polami input
            var emailInput = await _page.QuerySelectorAsync(_emailInputSelector);
            if (emailInput == null)
            {
                // Fallback: szukaj input[type='text']
                emailInput = await _page.QuerySelectorAsync("input[type='text']");
            }
            var passwordInput = await _page.QuerySelectorAsync(_passwordInputSelector);
            if (emailInput != null && passwordInput != null)
            {
                await emailInput.FillAsync(email);
                Console.WriteLine("Wypełniono pole email");
                await Task.Delay(500);
                
                await passwordInput.FillAsync(password);
                Console.WriteLine("Wypełniono pole hasła");
                await Task.Delay(500);
            }
            else
            {
                throw new Exception("Nie znaleziono pól email lub hasła");
            }
            
            // Zrzut ekranu po wypełnieniu formularza
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "login-form-filled.png" });
            
            // Szukamy przycisku logowania
            Console.WriteLine("Szukam przycisku logowania...");
            var loginButton = await _page.QuerySelectorAsync(_loginButtonSelector);
            
            if (loginButton == null)
            {
                throw new Exception("Nie znaleziono przycisku logowania");
            }
            
            // Klikamy przycisk logowania
            await loginButton.ClickAsync();
            Console.WriteLine("Kliknięto przycisk logowania");
            
            // Czekamy na przekierowanie lub komunikat o błędzie
            await Task.Delay(3000);
            
            // Zrzut ekranu po kliknięciu przycisku logowania
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "after-login-click.png" });
            
            // Sprawdzamy, czy użytkownik został przekierowany na stronę główną
            var currentUrl = _page.Url;
            Console.WriteLine($"Aktualny URL po logowaniu: {currentUrl}");
            
            if (currentUrl == $"{_baseUrl}/" || currentUrl.StartsWith($"{_baseUrl}/?"))
            {
                Console.WriteLine("Przekierowano na stronę główną - logowanie udane");
                return true;
            }
            
            // Sprawdzamy, czy wystąpił błąd logowania
            var errorElement = await _page.QuerySelectorAsync(_errorMessageSelector);
            if (errorElement != null)
            {
                var errorText = await errorElement.TextContentAsync();
                Console.WriteLine($"Znaleziono komunikat o błędzie: {errorText}");
                return false;
            }
            
            Console.WriteLine("Nie wykryto ani przekierowania, ani komunikatu o błędzie - sprawdzam zawartość strony");
            var html = await _page.ContentAsync();
            if (html.Contains("Wyloguj") || html.Contains("konto") || html.Contains("profil"))
            {
                Console.WriteLine("Znaleziono tekst wskazujący na zalogowanie");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas logowania: {ex.Message}");
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "login-error.png" });
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