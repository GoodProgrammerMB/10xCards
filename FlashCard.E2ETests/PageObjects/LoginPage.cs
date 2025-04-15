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
    private readonly string _loginButtonSelector = "button:has-text('Zaloguj')";
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
            var inputs = await _page.QuerySelectorAllAsync("input");
            if (inputs.Count >= 2)
            {
                // Szukamy pola email - jeśli jest, to prawdopodobnie już jesteśmy na zakładce logowania
                var emailInput = await _page.QuerySelectorAsync(_emailInputSelector);
                if (emailInput != null && await emailInput.IsVisibleAsync())
                {
                    Console.WriteLine("Już jesteśmy na zakładce logowania.");
                    return;
                }
            }
            
            // Szukamy i klikamy zakładkę logowania
            var buttons = await _page.QuerySelectorAllAsync("button");
            Console.WriteLine($"Znaleziono {buttons.Count} przycisków:");
            
            foreach (var button in buttons)
            {
                var text = await button.TextContentAsync();
                var isVisible = await button.IsVisibleAsync();
                Console.WriteLine($"Przycisk: '{text}', widoczny: {isVisible}");
                
                if (text != null && text.Contains("Logowanie") && isVisible)
                {
                    Console.WriteLine("Znaleziono przycisk Logowanie, klikam...");
                    await button.ClickAsync();
                    break;
                }
            }
            
            // Czekamy na załadowanie formularza
            Console.WriteLine("Oczekiwanie po kliknięciu zakładki logowania...");
            await Task.Delay(3000);
            
            // Zrzut ekranu dla diagnostyki
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "login-tab-clicked.png" });
            
            // Wypisujemy znalezione pola input
            inputs = await _page.QuerySelectorAllAsync("input");
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
            var inputs = await _page.QuerySelectorAllAsync("input");
            if (inputs.Count >= 2)
            {
                // Zakładamy, że pierwsze pole to email, drugie to hasło
                await inputs[0].FillAsync(email);
                Console.WriteLine("Wypełniono pole email");
                await Task.Delay(1000);
                
                await inputs[1].FillAsync(password);
                Console.WriteLine("Wypełniono pole hasła");
                await Task.Delay(1000);
            }
            else
            {
                // Metoda alternatywna z klawiaturą
                Console.WriteLine("Nie znaleziono pól, używam metody z klawiaturą");
                if (inputs.Count > 0)
                {
                    await inputs[0].ClickAsync();
                    await _page.Keyboard.TypeAsync(email);
                    await _page.Keyboard.PressAsync("Tab");
                    await _page.Keyboard.TypeAsync(password);
                }
                else
                {
                    throw new Exception("Nie znaleziono pól formularza");
                }
            }
            
            // Zrzut ekranu po wypełnieniu formularza
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "login-form-filled.png" });
            
            // Szukamy przycisku logowania
            Console.WriteLine("Szukam przycisku logowania...");
            var buttons = await _page.QuerySelectorAllAsync("button");
            var loginButton = null as IElementHandle;
            
            foreach (var button in buttons)
            {
                var text = await button.TextContentAsync();
                var isVisible = await button.IsVisibleAsync();
                Console.WriteLine($"Przycisk: '{text}', widoczny: {isVisible}");
                
                if (text != null && (text.Contains("Zaloguj") || text.Contains("Login")) && isVisible)
                {
                    loginButton = button;
                    break;
                }
            }
            
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