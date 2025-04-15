using Microsoft.Playwright;

namespace FlashCard.E2ETests.PageObjects;

/// <summary>
/// Reprezentuje stronę rejestracji w aplikacji.
/// </summary>
public class RegisterPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;
    
    // Zaktualizowane selektory do MudBlazor
    private readonly string _registrationTabSelector = "button:has-text('Rejestracja')";
    // Używamy bardziej ogólnych selektorów dla pól formularza
    private readonly string _emailInputSelector = "input[type='email']";
    private readonly string _passwordInputSelector = "input[type='password']";
    private readonly string _confirmPasswordInputSelector = "input[type='password']:nth-of-type(2)";
    private readonly string _registerButtonSelector = "button:has-text('Zarejestruj')";
    private readonly string _successMessageSelector = ".mud-snackbar";
    private readonly string _errorMessageSelector = ".mud-alert";
    
    public RegisterPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }
    
    /// <summary>
    /// Przechodzi do strony logowania/rejestracji i klika zakładkę rejestracji.
    /// </summary>
    public async Task GoToAsync()
    {
        // Przechodzimy na stronę główną
        await _page.GotoAsync(_baseUrl);
        await Task.Delay(2000); // Krókie opóźnienie po załadowaniu strony
        
        // Dodajemy logowanie dla debugowania
        Console.WriteLine($"Załadowano stronę: {_page.Url}");
        
        try 
        {
            // Czekamy z dłuższym timeout, aby strona się załadowała
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // Wykonujemy zrzut ekranu po załadowaniu strony
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "page-loaded.png" });
            
            // Sprawdzamy, czy strona zawiera zakładkę rejestracji
            var html = await _page.ContentAsync();
            Console.WriteLine($"Czy strona zawiera tekst 'Rejestracja': {html.Contains("Rejestracja")}");
            
            // Szukamy wszystkich przycisków z tekstem
            var buttons = await _page.QuerySelectorAllAsync("button");
            Console.WriteLine($"Znaleziono {buttons.Count} przycisków:");
            foreach (var button in buttons)
            {
                var text = await button.TextContentAsync();
                var isVisible = await button.IsVisibleAsync();
                Console.WriteLine($"Przycisk: '{text}', widoczny: {isVisible}");
                
                if (text != null && text.Contains("Rejestracja") && isVisible)
                {
                    Console.WriteLine("Znaleziono przycisk Rejestracja, klikam...");
                    await button.ClickAsync();
                    break;
                }
            }
            
            Console.WriteLine("Oczekiwanie 5 sekund po kliknięciu zakładki...");
            await Task.Delay(5000); // Dłuższe opóźnienie po kliknięciu zakładki
            
            // Wykonujemy zrzut ekranu po kliknięciu zakładki rejestracji
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "after-tab-click.png" });
            
            // Wyświetlamy wszystkie pola input na stronie dla diagnostyki
            var inputs = await _page.QuerySelectorAllAsync("input");
            Console.WriteLine($"Znaleziono {inputs.Count} pól input na stronie:");
            foreach (var input in inputs)
            {
                var type = await input.GetAttributeAsync("type");
                var id = await input.GetAttributeAsync("id");
                var name = await input.GetAttributeAsync("name");
                var isVisible = await input.IsVisibleAsync();
                Console.WriteLine($"Input: type={type}, id={id}, name={name}, widoczny: {isVisible}");
            }
            
            // Sukces - zakładamy, że przeszliśmy do zakładki rejestracji
            Console.WriteLine("Przejście do zakładki rejestracji zakończone.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas przechodzenia do zakładki rejestracji: {ex.Message}");
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "registration-tab-error.png" });
            throw;
        }
    }
    
    /// <summary>
    /// Wypełnia formularz rejestracji i klika przycisk "Zarejestruj się".
    /// </summary>
    /// <param name="email">Adres email do rejestracji</param>
    /// <param name="password">Hasło</param>
    /// <param name="confirmPassword">Potwierdzenie hasła</param>
    public async Task RegisterAsync(string email, string password, string confirmPassword)
    {
        Console.WriteLine($"Wypełnianie formularza rejestracji: {email}");
        
        try
        {
            // Wykonujemy zrzut ekranu przed wypełnianiem
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "before-filling-form.png" });
            
            // Wprowadzamy dane bezpośrednio za pomocą klawiatury
            Console.WriteLine("Próbuję wprowadzić email...");
            
            // Próba 1: Wpisz dane bezpośrednio w odpowiednie pola
            var inputs = await _page.QuerySelectorAllAsync("input");
            if (inputs.Count >= 3)
            {
                // Zakładamy, że pola są w kolejności: email, hasło, potwierdzenie hasła
                await inputs[0].FillAsync(email);
                Console.WriteLine("Wypełniono pole email");
                await Task.Delay(1000);
                
                await inputs[1].FillAsync(password);
                Console.WriteLine("Wypełniono pole hasła");
                await Task.Delay(1000);
                
                await inputs[2].FillAsync(confirmPassword);
                Console.WriteLine("Wypełniono pole potwierdzenia hasła");
                await Task.Delay(1000);
            }
            else
            {
                // Jeśli nie znaleziono wystarczającej liczby pól input, używamy metody klawiatury
                Console.WriteLine("Nie znaleziono wszystkich pól, używam metody klawiatury");
                
                // Próbujemy znaleźć jakiekolwiek pole input i kliknąć na nie
                if (inputs.Count > 0)
                {
                    await inputs[0].ClickAsync();
                    await _page.Keyboard.TypeAsync(email);
                    await _page.Keyboard.PressAsync("Tab");
                    await _page.Keyboard.TypeAsync(password);
                    await _page.Keyboard.PressAsync("Tab");
                    await _page.Keyboard.TypeAsync(confirmPassword);
                    Console.WriteLine("Wprowadzono dane za pomocą klawiatury");
                }
                else
                {
                    throw new Exception("Nie znaleziono żadnych pól input");
                }
            }
            
            // Wykonujemy zrzut ekranu po wypełnieniu formularza
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "after-filling-form.png" });
            
            // Szukamy przycisku rejestracji
            Console.WriteLine("Szukam przycisku rejestracji...");
            var buttons = await _page.QuerySelectorAllAsync("button");
            var registerButton = null as IElementHandle;
            
            foreach (var button in buttons)
            {
                var text = await button.TextContentAsync();
                var isVisible = await button.IsVisibleAsync();
                Console.WriteLine($"Przycisk: '{text}', widoczny: {isVisible}");
                
                if (text != null && (text.Contains("Zarejestruj") || text.Contains("Zapisz")) && isVisible)
                {
                    registerButton = button;
                    break;
                }
            }
            
            if (registerButton == null)
            {
                throw new Exception("Nie znaleziono przycisku rejestracji");
            }
            
            // Klikamy przycisk rejestracji
            await registerButton.ClickAsync();
            Console.WriteLine("Kliknięto przycisk rejestracji");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas wypełniania formularza rejestracji: {ex.Message}");
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "register-error.png" });
            throw;
        }
    }
    
    /// <summary>
    /// Sprawdza, czy rejestracja zakończyła się powodzeniem.
    /// </summary>
    /// <returns>True jeśli rejestracja się powiodła, false w przeciwnym wypadku</returns>
    public async Task<bool> IsRegistrationSuccessfulAsync()
    {
        try
        {
            // Dodajemy opóźnienie, aby dać czas na pojawienie się komunikatu
            await Task.Delay(2000);
            
            // Najpierw spróbujmy znaleźć powiadomienie (snackbar)
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "checking-registration-result.png" });
            
            // Szukamy elementów na stronie, które mogą zawierać informację o sukcesie
            var snackbars = await _page.QuerySelectorAllAsync(".mud-snackbar");
            foreach (var snackbar in snackbars)
            {
                var message = await snackbar.TextContentAsync();
                Console.WriteLine($"Znaleziono komunikat: {message}");
                if (message != null && message.Contains("pomyślnie"))
                {
                    return true;
                }
            }
            
            // Jeśli nie znaleziono snackbara, spróbujmy poszukać innych wskazówek
            var html = await _page.ContentAsync();
            if (html.Contains("pomyślnie") || html.Contains("success"))
            {
                Console.WriteLine("Znaleziono tekst o pomyślnej rejestracji w treści strony");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas sprawdzania wyniku rejestracji: {ex.Message}");
            
            // Sprawdźmy, czy jest komunikat o błędzie
            try
            {
                var errorMsg = await _page.TextContentAsync(_errorMessageSelector);
                if (errorMsg != null)
                {
                    Console.WriteLine($"Znaleziono komunikat o błędzie: {errorMsg}");
                }
            }
            catch { }
            
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "registration-result-error.png" });
            return false;
        }
    }
    
    /// <summary>
    /// Pobiera komunikat o błędzie, jeśli rejestracja się nie powiodła.
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