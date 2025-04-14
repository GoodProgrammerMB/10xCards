Frontend

MudBlazor Server – dodaj tę bibliotekę komponentów inspirowanych Material Design, aby szybko budować nowoczesne i estetyczne interfejsy użytkownika. Instalacja pakietu NuGet w projekcie klienta oraz dodanie przestrzeni nazw w pliku _imports.razor zapewni dostęp do gotowych komponentów, takich jak przyciski, tabele czy karty.
- Utwórz nowy projekt Blazor Server App (dotnet new blazorserver -n MyMudBlazorApp)
- Zainstaluj pakiet MudBlazor
- W pliku _Imports.razor dodaj dyrektywę:
@using MudBlazor
aby mieć dostęp do komponentów MudBlazor w całym projekc
- W pliku Program.cs  zarejestruj usługi MudBlazor, dodając:
builder.Services.AddMudServices();
-W pliku _Host.cshtml  uzupełnij sekcję <head> i/lub <body> o link do arkusza stylów oraz odwołanie do skryptu MudBlazor:
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
...
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
co pozwoli na prawidłowe ładowanie stylów i funkcjonalności komponentów
- Zmodyfikuj układ interfejsu użytkownika (np. plik MainLayout.razor w folderze Shared), aby otoczyć stronę dostawcami MudBlazor, wstawiając takie komponenty jak:
<MudThemeProvider>, <MudDialogProvider> i <MudSnackbarProvider> – to umożliwi korzystanie z globalnych ustawień stylów i powiadomień w całej aplikacji

Backend i Baza Danych
ASP.NET Core – wykorzystaj ten framework do budowy API oraz logiki biznesowej, co umożliwi skalowalną i bezpieczną obsługę po stronie serwera.

minimal API w C# - z wykożystaniem wbudowanego mechanizmu ASP.NET Core Identity

Entity Framework Core (EF Core) – zastosuj EF Core jako narzędzie ORM, aby uprościć dostęp do danych oraz zarządzanie operacjami na bazie, pamiętając o specyficznych wymaganiach dotyczących kontekstu i cykli życia DbContext w aplikacjach Blazor Server.

Ms SQL Server – wykorzystaj Ms SQL Server jako system bazodanowy, który zapewnia wysoką niezawodność i skalowalność oraz jest dobrze wspierany przez EF Core. W pliku appsettings.json określ odpowiednią konfigurację połączenia, dostosowaną do środowiska produkcyjnego lub deweloperskiego.

Komunikacja z Modelami AI
Openrouter.ai – zaimplementuj dedykowany serwis, który korzysta z HttpClient w ASP.NET Core, aby komunikować się z API Openrouter.ai. Dzięki temu Twoja aplikacja będzie mogła wysyłać zapytania do modeli AI i odbierać rezultaty, integrując funkcjonalności uczenia maszynowego lub przetwarzania języka naturalnego.

CI/CD i Hosting
GitHub Actions – skonfiguruj pipeline, który automatycznie buduje, testuje i wdraża aplikację. Plik YAML określi kroki budowy obrazu Docker, uruchomienie testów oraz publikację artefaktów, co usprawnia cykl rozwoju oprogramowania.

DigitalOcean – wybierz DigitalOcean do hostingu aplikacji, korzystając z opcji wdrożeń kontenerowych lub maszyn wirtualnych. DigitalOcean umożliwia skalowanie aplikacji oraz szybkie wdrożenia dzięki zautomatyzowanym workflowom, co stanowi doskonałe uzupełnienie dla konfiguracji CI/CD przeprowadzanej przez GitHub Actions.

Testowanie
Testy Jednostkowe (Unit Tests) – zaimplementuj testy jednostkowe przy użyciu frameworka xUnit oraz biblioteki Moq do mockowania zależności. Skoncentruj się na izolowanym testowaniu komponentów backendu, w tym serwisów, logiki walidacji oraz helperów. Testy te powinny być częścią procesu CI/CD, zapewniając szybką informację zwrotną o jakości kodu.
- Zainstaluj pakiety NuGet xUnit, xUnit.runner.visualstudio oraz Moq
- Utwórz projekt testowy (dotnet new xunit -n FlashCard.Tests)
- Napisz testy pokrywające kluczowe komponenty aplikacji, szczególnie serwisy komunikujące się z OpenRouter.ai

Testy End-to-End (E2E) – wykorzystaj Playwright do automatyzacji testów interfejsu użytkownika, sprawdzając przepływy użytkownika z perspektywy frontendu. Testy te powinny weryfikować, czy cały system działa poprawnie, obejmując interakcje z UI, API oraz bazą danych.
- Zainstaluj Playwright za pomocą .NET CLI lub NuGet
- Utwórz projekt testów E2E (dotnet new nunit -n FlashCard.E2ETests)
- Skonfiguruj Playwright do obsługi Blazor Server (uwzględniając specyfikę SignalR)
- Zautomatyzuj kluczowe przepływy użytkownika, takie jak rejestracja, logowanie, generowanie fiszek czy zarządzanie nimi

Testy API – wykorzystaj HttpClient w testach integracyjnych do weryfikacji poprawności działania endpointów API, w tym walidacji danych, autoryzacji oraz obsługi błędów. Testy te są kluczowe dla zapewnienia niezawodności backendu i powinny być uruchamiane w środowisku testowym zbliżonym do produkcyjnego.
- Zaimplementuj klasę testową korzystającą z WebApplicationFactory<Program> do hostowania API w testach
- Napisz testy dla wszystkich endpointów API, sprawdzając różne scenariusze (poprawne dane, niepoprawne dane, autoryzacja)

Podsumowując, ten stack technologiczny łączy interaktywny frontend oparty na Blazor Server oraz MudBlazor z niezawodnym backendem ASP.NET Core wykorzystującym EF Core i Ms SQL Server. Dzięki integracji z Openrouter.ai aplikacja może komunikować się z modelami AI, a zautomatyzowany proces CI/CD w GitHub Actions oraz hosting na DigitalOcean zapewniają ciągłość wdrożeń i skalowalność rozwiązania. Kompleksowa strategia testowania z wykorzystaniem xUnit, Moq i Playwright gwarantuje wysoką jakość kodu i niezawodne działanie aplikacji.