using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlashCard.E2ETests.Fixtures;
using System.Threading.Tasks;

namespace FlashCard.E2ETests;

/// <summary>
/// Klasa zawierająca metody inicjalizacyjne, które zostaną wywołane przed uruchomieniem testów.
/// </summary>
[TestClass]
public class PlaywrightSetup
{
    /// <summary>
    /// Metoda wywoływana raz przed uruchomieniem wszystkich testów w assembly.
    /// Instaluje zależności Playwright (przeglądarki).
    /// </summary>
    [AssemblyInitialize]
    public static async Task AssemblyInitializeAsync(TestContext context)
    {
        // Instalujemy przeglądarki potrzebne do testów Playwright
        await PlaywrightFixture.InstallBrowsersAsync();
    }
} 