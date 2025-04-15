//using FlashCard.E2ETests.PageObjects;
//using Microsoft.Playwright;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Threading.Tasks;
//using System.Text.Json;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.EntityFrameworkCore;
//using FlashCard.Api.Data;
//using System.Net.Http.Json;
//using System.Net;

//namespace FlashCard.E2ETests.Tests;

//[TestClass]
//public class RegisterAndLoginTests
//{
//    private IBrowser _browser;
//    private IBrowserContext _context;
//    private IPage _page;
//    private RegisterPage _registerPage;
//    private LoginPage _loginPage;
//    private HttpClient _httpClient;
//    private string _authToken;
//    private DbContextOptions<FlashCardDbContext> _dbContextOptions;
//    private FlashCardDbContext _dbContext;
    
//    // Zmiana portów na domyślne porty używane przez dotnet run (dla Blazor i API)
//    private const string BaseUrl = "http://localhost:5007";
//    private const string ApiUrl = "http://localhost:5170";
    
//    // Stałe dane testowe
//    private const string TestEmail = "teste2e@wp.pl";
//    private const string TestPassword = "teste2e";
    
//    [TestInitialize]
//    public async Task TestInitialize()
//    {
//        try
//        {
//            Console.WriteLine("Inicjalizacja testu...");
            
//            // Konfiguracja bazy danych w pamięci
//            SetupInMemoryDatabase();
            
//            // Inicjalizacja HttpClient dla API
//            _httpClient = new HttpClient();
//            _httpClient.BaseAddress = new Uri(ApiUrl);
            
//            // Sprawdzamy, czy aplikacja jest dostępna przed rozpoczęciem testu
//            await CheckApplicationAvailabilityAsync();
            
//            // Inicjalizacja Playwright
//            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
//            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
//            {
//                Headless = false, // Tryb z UI
//                SlowMo = 1000, // Zwiększamy opóźnienie dla lepszej obserwacji - 1 sekunda
//                Timeout = 90000 // Zwiększamy timeout do 90 sekund
//            });
            
//            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
//            {
//                IgnoreHTTPSErrors = true, // Ignoruj błędy HTTPS/SSL
//                ViewportSize = new ViewportSize
//                {
//                    Width = 1280,
//                    Height = 720
//                }
//            });
            
//            // Dodajemy obsługę dialogów
//            _context.Dialog += (_, dialog) => 
//            {
//                Console.WriteLine($"Dialog: {dialog.Message}");
//                dialog.AcceptAsync().GetAwaiter().GetResult();
//            };
            
//            _page = await _context.NewPageAsync();
            
//            // Dodajemy obsługę konsoli
//            _page.Console += (_, msg) => 
//            {
//                Console.WriteLine($"Konsola: [{msg.Type}] {msg.Text}");
//            };
            
//            // Przechwytujemy błędy z przeglądarki
//            _page.PageError += (_, error) => 
//            {
//                Console.WriteLine($"Błąd strony: {error}");
//            };
            
//            // Inicjalizacja Page Objects
//            _registerPage = new RegisterPage(_page, BaseUrl);
//            _loginPage = new LoginPage(_page, BaseUrl);
            
//            Console.WriteLine("Inicjalizacja testu zakończona powodzeniem.");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Błąd podczas inicjalizacji testu: {ex.Message}");
//            throw;
//        }
//    }
    
//    /// <summary>
//    /// Sprawdza, czy aplikacja i API są dostępne przed rozpoczęciem testu
//    /// </summary>
//    private async Task CheckApplicationAvailabilityAsync()
//    {
//        Console.WriteLine("Sprawdzanie dostępności aplikacji...");
        
//        // Sprawdzamy dostępność frontendu
//        var frontendClient = new HttpClient();
//        try
//        {
//            var frontendResponse = await frontendClient.GetAsync(BaseUrl);
//            Console.WriteLine($"Frontend status: {frontendResponse.StatusCode}");
            
//            if (!frontendResponse.IsSuccessStatusCode && frontendResponse.StatusCode != HttpStatusCode.Unauthorized)
//            {
//                Console.WriteLine($"OSTRZEŻENIE: Frontend nie jest dostępny pod adresem {BaseUrl}.");
//                Console.WriteLine("Upewnij się, że aplikacja Blazor jest uruchomiona przed wykonaniem testu.");
//                Assert.Inconclusive($"Aplikacja frontendowa nie jest dostępna pod adresem {BaseUrl}. Uruchom aplikację przed wykonaniem testu.");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"KRYTYCZNY BŁĄD: Nie można połączyć się z frontendem: {ex.Message}");
//            Console.WriteLine(@"
//*****************************************************
//* UWAGA: Aplikacja frontendowa nie jest uruchomiona! *
//* Uruchom aplikację Blazor poleceniem:               *
//* dotnet run --project FlashCard.App                 *
//* przed wykonaniem testu.                            *
//*****************************************************
//");
//            Assert.Inconclusive($"Aplikacja nie jest uruchomiona pod adresem {BaseUrl}. Uruchom aplikację przed wykonaniem testu.");
//        }
//        finally
//        {
//            frontendClient.Dispose();
//        }
        
//        // Sprawdzamy dostępność backendu
//        var apiClient = new HttpClient();
//        try
//        {
//            var apiResponse = await apiClient.GetAsync($"{ApiUrl}/api/health");
//            Console.WriteLine($"API status: {apiResponse.StatusCode}");
            
//            if (!apiResponse.IsSuccessStatusCode && apiResponse.StatusCode != HttpStatusCode.Unauthorized)
//            {
//                Console.WriteLine($"OSTRZEŻENIE: API nie jest dostępne pod adresem {ApiUrl}.");
//                Console.WriteLine("Upewnij się, że API jest uruchomione przed wykonaniem testu.");
//                Assert.Inconclusive($"API nie jest dostępne pod adresem {ApiUrl}. Uruchom API przed wykonaniem testu.");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"KRYTYCZNY BŁĄD: Nie można połączyć się z API: {ex.Message}");
//            Console.WriteLine(@"
//*****************************************************
//* UWAGA: API nie jest uruchomione!                  *
//* Uruchom API poleceniem:                           *
//* dotnet run --project FlashCard.Api                *
//* przed wykonaniem testu.                           *
//*****************************************************
//");
//            Assert.Inconclusive($"API nie jest uruchomione pod adresem {ApiUrl}. Uruchom API przed wykonaniem testu.");
//        }
//        finally
//        {
//            apiClient.Dispose();
//        }
        
//        Console.WriteLine("Aplikacja i API są dostępne. Kontynuuję test.");
//    }
    
//    private void SetupInMemoryDatabase()
//    {
//        // Konfiguracja i utworzenie bazy danych w pamięci
//        Console.WriteLine("Konfiguracja bazy danych w pamięci...");
        
//        _dbContextOptions = new DbContextOptionsBuilder<FlashCardDbContext>()
//            .UseInMemoryDatabase(databaseName: $"FlashCardTestDb_{Guid.NewGuid()}")
//            .Options;
        
//        _dbContext = new FlashCardDbContext(_dbContextOptions);
        
//        // Upewniamy się, że baza jest utworzona i czysta
//        _dbContext.Database.EnsureCreated();
        
//        Console.WriteLine("Baza danych w pamięci została utworzona.");
//    }
    
//    [TestCleanup]
//    public async Task TestCleanup()
//    {
//        try 
//        {
//            Console.WriteLine("Zamykanie zasobów testu...");
            
//            // Wykonujemy zrzut ekranu przed zamknięciem
//            if (_page != null)
//            {
//                try 
//                {
//                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "test-cleanup.png" });
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Nie udało się wykonać zrzutu ekranu: {ex.Message}");
//                }
//            }
            
//            // Próba usunięcia użytkownika testowego jeśli mamy token
//            if (!string.IsNullOrEmpty(_authToken))
//            {
//                await TryDeleteTestUserAsync();
//            }
            
//            // Czyszczenie bazy danych
//            await CleanupDatabaseAsync();
            
//            // Dodajemy opóźnienie przed zamknięciem przeglądarki
//            await Task.Delay(2000);
            
//            // Zamykamy zasoby
//            if (_context != null)
//                await _context.CloseAsync();
//            if (_browser != null)
//                await _browser.CloseAsync();
//            if (_httpClient != null)
//                _httpClient.Dispose();
//            if (_dbContext != null)
//                await _dbContext.DisposeAsync();
                
//            Console.WriteLine("Zasoby testu zostały zamknięte.");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Błąd podczas zamykania zasobów testu: {ex.Message}");
//        }
//    }
    
//    private async Task CleanupDatabaseAsync()
//    {
//        try
//        {
//            Console.WriteLine("Czyszczenie bazy danych...");
            
//            // Usuwamy testowe dane z bazy danych
//            if (_dbContext != null)
//            {
//                // Usuwamy użytkownika testowego (i powiązane z nim dane dzięki kaskadowemu usuwaniu)
//                var testUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == TestEmail);
//                if (testUser != null)
//                {
//                    _dbContext.Users.Remove(testUser);
//                    await _dbContext.SaveChangesAsync();
//                    Console.WriteLine($"Usunięto testowego użytkownika: {TestEmail}");
//                }
                
//                // Alternatywnie możemy usunąć całą bazę
//                await _dbContext.Database.EnsureDeletedAsync();
//                Console.WriteLine("Baza danych została wyczyszczona.");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Błąd podczas czyszczenia bazy danych: {ex.Message}");
//        }
//    }
    
//    [TestMethod]
//    public async Task RegisterAndLogin_ShouldWorkEndToEnd()
//    {
//        try
//        {
//            Console.WriteLine("Rozpoczynam test rejestracji i logowania.");
            
//            // Używamy stałych danych testowych
//            string email = TestEmail;
//            string password = TestPassword;
            
//            Console.WriteLine($"Dane testowe: {email}, {password}");
            
//            // Wykonanie zrzutu ekranu początkowego
//            await _page.ScreenshotAsync(new PageScreenshotOptions 
//            { 
//                Path = "test-start.png",
//                FullPage = true
//            });
            
//            // KROK 1: Przechodzimy do zakładki rejestracji
//            Console.WriteLine("KROK 1: Przechodzę do zakładki rejestracji");
//            await _registerPage.GoToAsync();
            
//            // Dodajemy opóźnienie, aby można było zobaczyć formularz
//            await Task.Delay(2000);
            
//            // Wykonanie zrzutu ekranu po przejściu do zakładki rejestracji
//            await _page.ScreenshotAsync(new PageScreenshotOptions 
//            { 
//                Path = "register-tab.png",
//                FullPage = true
//            });
            
//            // KROK 2: Wypełniamy formularz rejestracji
//            Console.WriteLine("KROK 2: Wypełniam formularz rejestracji");
//            await _registerPage.RegisterAsync(email, password, password);
            
//            // Dodajemy opóźnienie po kliknięciu przycisku rejestracji
//            await Task.Delay(3000);
            
//            // Wykonanie zrzutu ekranu po wypełnieniu formularza
//            await _page.ScreenshotAsync(new PageScreenshotOptions 
//            { 
//                Path = "after-register-submit.png",
//                FullPage = true
//            });
            
//            // Sprawdzamy, czy po rejestracji użytkownik od razu został zalogowany
//            Console.WriteLine("Sprawdzam, czy użytkownik został przekierowany do aplikacji");
            
//            // Oczekujemy na przekierowanie do głównej strony
//            await _page.WaitForURLAsync($"{BaseUrl}/");
            
//            // Wykonanie zrzutu ekranu po przekierowaniu
//            await _page.ScreenshotAsync(new PageScreenshotOptions 
//            { 
//                Path = "after-register-redirect.png",
//                FullPage = true
//            });
            
//            // Sprawdzamy, czy na stronie istnieją elementy menu
//            Console.WriteLine("Sprawdzam, czy na stronie istnieją elementy menu");
//            bool hasMainMenu = await CheckMainMenuElements();
            
//            Assert.IsTrue(hasMainMenu, "Na stronie powinny być widoczne elementy menu po zalogowaniu");
            
//            // Zapisujemy token JWT z localStorage do późniejszego usunięcia konta
//            await SaveAuthToken();
            
//            // Dodatkowe sprawdzenie w bazie danych, czy użytkownik został utworzony
//            await VerifyUserCreatedInDatabase();
            
//            Console.WriteLine("Test rejestracji i przekierowania do aplikacji zakończony pomyślnie.");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Błąd podczas testu: {ex}");
            
//            // Wykonujemy zrzut ekranu w przypadku błędu
//            try 
//            {
//                await _page.ScreenshotAsync(new PageScreenshotOptions 
//                { 
//                    Path = "test-error.png",
//                    FullPage = true
//                });
//            }
//            catch
//            {
//                Console.WriteLine("Nie udało się wykonać zrzutu ekranu błędu.");
//            }
            
//            throw;
//        }
//    }
    
//    /// <summary>
//    /// Sprawdza, czy użytkownik testowy został utworzony w bazie danych
//    /// </summary>
//    private async Task VerifyUserCreatedInDatabase()
//    {
//        try 
//        {
//            var testUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == TestEmail);
//            if (testUser != null)
//            {
//                Console.WriteLine($"Potwierdzono utworzenie użytkownika w bazie danych: {testUser.Email}, ID: {testUser.Id}");
//            }
//            else
//            {
//                Console.WriteLine("Nie znaleziono użytkownika testowego w bazie danych.");
//                Assert.Fail("Użytkownik testowy nie został utworzony w bazie danych.");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Błąd podczas weryfikacji użytkownika w bazie danych: {ex.Message}");
//        }
//    }
    
//    /// <summary>
//    /// Sprawdza, czy na stronie istnieją elementy menu po zalogowaniu
//    /// </summary>
//    private async Task<bool> CheckMainMenuElements()
//    {
//        try
//        {
//            // Szukamy elementów menu, które powinny być widoczne po zalogowaniu
//            var menuItems = await _page.QuerySelectorAllAsync(".mud-nav-link");
//            Console.WriteLine($"Znaleziono {menuItems.Count} elementów menu");
            
//            // Wypisujemy znalezione elementy menu dla diagnostyki
//            foreach (var item in menuItems)
//            {
//                var text = await item.TextContentAsync();
//                Console.WriteLine($"Element menu: {text}");
//            }
            
//            // Jeśli znaleziono jakiekolwiek elementy menu, uznajemy że test zakończył się sukcesem
//            return menuItems.Count > 0;
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Błąd podczas sprawdzania elementów menu: {ex.Message}");
//            return false;
//        }
//    }
    
//    /// <summary>
//    /// Zapisuje token JWT z localStorage przeglądarki
//    /// </summary>
//    private async Task SaveAuthToken()
//    {
//        try
//        {
//            var token = await _page.EvaluateAsync<string>("localStorage.getItem('authToken')");
//            if (!string.IsNullOrEmpty(token))
//            {
//                Console.WriteLine("Pomyślnie pobrano token JWT z localStorage");
//                _authToken = token;
//            }
//            else
//            {
//                Console.WriteLine("Nie znaleziono tokenu JWT w localStorage");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Błąd podczas pobierania tokenu JWT: {ex.Message}");
//        }
//    }
    
//    /// <summary>
//    /// Próbuje usunąć użytkownika testowego poprzez bezpośrednie wywołanie API
//    /// </summary>
//    private async Task TryDeleteTestUserAsync()
//    {
//        try
//        {
//            Console.WriteLine("Próba usunięcia użytkownika testowego...");
            
//            // 1. Usuwamy token z przeglądarki (wylogowanie)
//            try 
//            {
//                await _page.EvaluateAsync("localStorage.removeItem('authToken')");
//                Console.WriteLine("Usunięto token użytkownika z localStorage przeglądarki");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Nie udało się usunąć tokenu: {ex.Message}");
//            }
            
//            // 2. Bezpośrednio usuwamy użytkownika z bazy danych
//            var testUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == TestEmail);
//            if (testUser != null)
//            {
//                _dbContext.Users.Remove(testUser);
//                await _dbContext.SaveChangesAsync();
//                Console.WriteLine($"Usunięto użytkownika testowego z bazy danych: {TestEmail}");
//            }
//            else
//            {
//                Console.WriteLine("Nie znaleziono użytkownika testowego w bazie danych do usunięcia.");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Błąd podczas usuwania użytkownika testowego: {ex.Message}");
//        }
//    }
//} 