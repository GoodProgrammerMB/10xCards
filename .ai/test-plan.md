Doskonale! Analizując dostarczone informacje o projekcie "10x-cards", przygotowałem kompleksowy plan testów, uwzględniając specyfikę technologiczną i funkcjonalną aplikacji.

**Plan Testów dla Projektu "10x-cards"**

**Wersja:** 1.0
**Data:** 2024-05-22
**Autor:** [Twoje Imię/Nazwisko], Doświadczony Inżynier QA

**Spis Treści:**

1.  Wprowadzenie
2.  Zakres Testów
    *   2.1. Funkcjonalności Włączone w Zakres (In Scope)
    *   2.2. Funkcjonalności Wyłączone z Zakresu (Out of Scope)
3.  Cele Testów
4.  Strategia Testowania
    *   4.1. Poziomy Testów
    *   4.2. Typy Testów
    *   4.3. Podejście do Automatyzacji
5.  Środowiska Testowe
6.  Zasoby i Role
7.  Harmonogram (Orientacyjny)
8.  Kryteria Wejścia i Wyjścia
    *   8.1. Kryteria Wejścia
    *   8.2. Kryteria Wyjścia
9.  Metryki Testowe
10. Dostarczane Artefakty
11. Ryzyka i Założenia
12. Scenariusze Testowe (Wysokopoziomowe)
    *   12.1. Autentykacja i Autoryzacja Użytkowników
    *   12.2. Zarządzanie Fiszkami (Manualne - CRUD)
    *   12.3. Generowanie Fiszek z Wykorzystaniem AI
    *   12.4. Zapisywanie Wygenerowanych Fiszek (Batch Save)
    *   12.5. Testy API (Bezpośrednie)
    *   12.6. Testy Interfejsu Użytkownika (Frontend - Blazor)
    *   12.7. Testy Bazy Danych
    *   12.8. Testy Bezpieczeństwa
    *   12.9. Testy Wydajności
    *   12.10. Testy Konfiguracji
    *   12.11. (MVP) Integracja z Systemem Powtórek (Spaced Repetition)
    *   12.12. (MVP) Analityka
13. Narzędzia
14. Zarządzanie Defektami

---

**1. Wprowadzenie**

Niniejszy dokument opisuje plan testów dla aplikacji webowej "10x-cards". Celem projektu jest stworzenie narzędzia do efektywnego tworzenia i zarządzania fiszkami edukacyjnymi, z wykorzystaniem AI do automatycznego generowania sugestii oraz mechanizmów powtórek (spaced repetition). Plan ten określa zakres, podejście, zasoby i harmonogram działań testowych mających na celu zapewnienie jakości i zgodności aplikacji z wymaganiami MVP (Minimum Viable Product).

**2. Zakres Testów**

**2.1. Funkcjonalności Włączone w Zakres (In Scope - Zgodnie z MVP)**

*   **Autentykacja Użytkowników:** Rejestracja, logowanie, zarządzanie sesją (token JWT).
*   **Automatyczne Generowanie Fiszek:** Wklejanie tekstu źródłowego, wysyłanie żądania do API (`/api/generations`), przetwarzanie odpowiedzi z AI (OpenRouter.ai), prezentacja sugestii fiszek.
*   **Ręczne Zarządzanie Fiszkami:** Tworzenie, odczyt (lista fiszek użytkownika), edycja i usuwanie fiszek (CRUD) poprzez interfejs użytkownika i API (`/api/flashcards`).
*   **Zapisywanie Wygenerowanych Fiszek:** Mechanizm zapisywania wybranych lub wszystkich wygenerowanych fiszek (`/api/flashcards/batch`).
*   **Integracja z Systemem Powtórek (Spaced Repetition - MVP):** Podstawowe planowanie przeglądu fiszek (szczegóły implementacji do ustalenia/zweryfikowania).
*   **Analityka (MVP):** Podstawowe śledzenie statystyk generowania i użycia fiszek (szczegóły implementacji do ustalenia/zweryfikowania).
*   **API Backendu (ASP.NET Core Minimal API):** Testowanie wszystkich endpointów pod kątem funkcjonalności, walidacji, obsługi błędów i bezpieczeństwa.
*   **Interfejs Użytkownika (Blazor Server + MudBlazor):** Testowanie komponentów, przepływów użytkownika, responsywności (podstawowej), interakcji.
*   **Baza Danych (MS SQL Server + EF Core):** Integralność danych, poprawność schematu, migracje, relacje, automatyczne ustawianie `CreatedAt`/`UpdatedAt`.
*   **Konfiguracja:** Poprawność ładowania i walidacji ustawień (DB Connection String, JWT, OpenRouter).
*   **Obsługa Błędów:** Zarówno na poziomie API, jak i UI (np. błędy walidacji, błędy komunikacji z AI, błędy serwera).

**2.2. Funkcjonalności Wyłączone z Zakresu (Out of Scope - dla MVP)**

*   Zaawansowane algorytmy spaced repetition.
*   Funkcje grywalizacji (Gamification).
*   Wsparcie dla aplikacji mobilnych.
*   Importowanie wielu formatów dokumentów (np. PDF, DOCX).
*   Publiczny dostęp do API.
*   Rozbudowane systemy powiadomień.
*   Zaawansowane testy obciążeniowe i stress testy (poza podstawowymi testami wydajności API).
*   Testy kompatybilności z szeroką gamą przeglądarek i ich starszych wersji (skupienie na najnowszych wersjach Chrome, Firefox, Edge).
*   Testy CI/CD pipeline (chyba że zostaną udostępnione środowiska).

**3. Cele Testów**

*   Weryfikacja, czy wszystkie funkcjonalności zdefiniowane w zakresie MVP działają zgodnie z oczekiwaniami.
*   Identyfikacja i raportowanie defektów aplikacji.
*   Ocena stabilności i niezawodności aplikacji.
*   Zapewnienie, że mechanizmy bezpieczeństwa (autentykacja, autoryzacja) są poprawnie zaimplementowane.
*   Weryfikacja poprawnej integracji pomiędzy frontendem (Blazor App), backendem (API) i usługami zewnętrznymi (OpenRouter.ai).
*   Ocena podstawowej użyteczności i doświadczenia użytkownika (UX).
*   Zapewnienie integralności danych w bazie danych.
*   Potwierdzenie, że aplikacja spełnia kryteria wyjścia przed wdrożeniem MVP.

**4. Strategia Testowania**

Zastosowane zostanie wielopoziomowe podejście do testowania, obejmujące różne typy testów.

**4.1. Poziomy Testów**

*   **Testy Jednostkowe (Unit Tests):** (Zakładając istnienie/rozwój na podstawie `GenerationServiceTests.cs` i zależności Moq/xUnit) Skupienie na izolowanych komponentach backendu (serwisy, logika walidacji, helpers). Deweloperzy są głównymi odpowiedzialnymi, QA może wspierać w definiowaniu przypadków.
*   **Testy Integracyjne (Integration Tests):** Testowanie interakcji pomiędzy komponentami backendu, np. Controller <-> Service <-> DbContext. Testowanie integracji z OpenRouter API (możliwe z użyciem mocków lub dedykowanego klucza testowego).
*   **Testy API (API Tests):** Bezpośrednie testowanie endpointów API (`FlashCard.Api`) za pomocą narzędzi typu Postman/Insomnia lub zautomatyzowanych skryptów. Weryfikacja kontraktów API, logiki biznesowej, autentykacji/autoryzacji, obsługi błędów.
*   **Testy Systemowe/End-to-End (E2E Tests):** Testowanie kompletnych przepływów użytkownika z perspektywy interfejsu (Blazor App), obejmujące interakcję UI -> API -> Baza Danych -> UI. Np. Rejestracja -> Logowanie -> Wygenerowanie fiszek -> Zapisanie -> Wyświetlenie na liście.

**4.2. Typy Testów**

*   **Testy Funkcjonalne:** Weryfikacja, czy funkcje aplikacji działają zgodnie z wymaganiami (opisane w sekcji Scenariusze Testowe).
*   **Testy Bezpieczeństwa:** Sprawdzanie mechanizmów autentykacji (JWT), autoryzacji (dostęp do zasobów tylko dla właściciela), walidacji danych wejściowych (zapobieganie np. SQL Injection - choć EF Core pomaga, XSS w UI), ochrona kluczy API (uwaga na klucze w `appsettings.json`!).
*   **Testy Wydajności (Podstawowe):** Mierzenie czasów odpowiedzi kluczowych endpointów API (zwłaszcza `/api/generations` i `/api/flashcards/batch`) pod umiarkowanym obciążeniem. Ocena czasu odpowiedzi AI.
*   **Testy Użyteczności (Usability Tests):** Ocena intuicyjności interfejsu, łatwości nawigacji i ogólnego doświadczenia użytkownika podczas wykonywania kluczowych zadań.
*   **Testy Walidacji:** Sprawdzanie poprawności walidacji danych wejściowych na poziomie API (modele DTO) i UI (formularze).
*   **Testy Dymne (Smoke Tests):** Szybki zestaw testów wykonywanych po każdej nowej budowie (buildzie) w celu weryfikacji, czy kluczowe funkcjonalności działają i można przystąpić do bardziej szczegółowych testów.
*   **Testy Regresji:** Ponowne wykonanie wybranych (lub wszystkich) testów po wprowadzeniu zmian w kodzie (naprawa błędów, nowe funkcje) w celu upewnienia się, że zmiany nie wprowadziły nowych błędów w istniejących funkcjonalnościach.

**4.3. Podejście do Automatyzacji**

*   **API Tests:** Wysoki priorytet dla automatyzacji. Testy API są stabilne i kluczowe dla weryfikacji logiki backendu. Można użyć narzędzi jak `HttpClient` w testach C# (integracyjnych/API) lub zewnętrznych narzędzi jak Postman (z kolekcjami i skryptami).
*   **E2E Tests:** Średni priorytet dla MVP. Można zautomatyzować kluczowe przepływy za pomocą narzędzi jak Playwright lub Selenium. Ze względu na Blazor Server, należy zwrócić uwagę na specyfikę interakcji (SignalR).
*   **Unit/Integration Tests:** Odpowiedzialność deweloperów, ale QA powinno mieć wgląd w pokrycie kodu testami.
*   **Pozostałe:** Testy użyteczności, eksploracyjne, bezpieczeństwa (częściowo) będą wykonywane manualnie.

**5. Środowiska Testowe**

*   **Lokalne (Development):** Używane przez deweloperów i QA do bieżącego testowania. Wymaga lokalnej instancji MS SQL Server, Node.js, .NET SDK oraz *testowego* klucza API OpenRouter.ai.
*   **Testowe/Staging:** Dedykowane środowisko, możliwie zbliżone do produkcyjnego (np. na DigitalOcean, jeśli to możliwe). Powinno mieć własną bazę danych i konfigurację (w tym klucz API OpenRouter). Idealne do testów regresji, E2E i UAT (User Acceptance Testing), jeśli dotyczy. Konfiguracja CORS musi uwzględniać URL tego środowiska.
*   **Produkcyjne:** Testy na produkcji ograniczone do minimalnych testów dymnych po wdrożeniu (jeśli polityka na to pozwala).

**6. Zasoby i Role**

*   **Inżynier QA:** Odpowiedzialny za stworzenie i wykonanie planu testów, projektowanie przypadków testowych (manualnych i automatycznych), raportowanie defektów, komunikację z zespołem deweloperskim.
*   **Deweloperzy:** Odpowiedzialni za testy jednostkowe, naprawę zgłoszonych defektów, wsparcie QA w diagnozowaniu problemów.
*   **Project Manager/Product Owner:** (Jeśli dotyczy) Definiowanie wymagań, priorytetyzacja defektów, akceptacja wyników testów.

**7. Harmonogram (Orientacyjny)**

Testowanie powinno odbywać się równolegle do procesu deweloperskiego, zgodnie z metodyką zwinną (jeśli taka jest stosowana).

*   **Faza Rozwoju Funkcjonalności:** Testowanie poszczególnych funkcji w miarę ich dostarczania (testy funkcjonalne, API, integracyjne).
*   **Faza Stabilizacji/Przed MVP:** Intensywne testy regresji, testy E2E, testy bezpieczeństwa, testy wydajnościowe, testy użyteczności.
*   **Po Wdrożeniu MVP:** Testy dymne na produkcji, monitorowanie.

Szczegółowy harmonogram zależy od planu rozwoju projektu.

**8. Kryteria Wejścia i Wyjścia**

**8.1. Kryteria Wejścia (Rozpoczęcie Testów Systemowych/E2E)**

*   Zakończenie implementacji kluczowych funkcjonalności MVP.
*   Dostępność stabilnej wersji aplikacji na środowisku testowym.
*   Pomyślne przejście podstawowych testów dymnych.
*   Dostępność dokumentacji (jeśli istnieje) lub jasne zrozumienie wymagań.
*   Poprawna konfiguracja środowiska testowego (DB, API Keys).

**8.2. Kryteria Wyjścia (Zakończenie Testów dla MVP)**

*   Wykonanie wszystkich zaplanowanych przypadków testowych dla zakresu MVP.
*   Brak otwartych defektów krytycznych (blokujących) i wysokich.
*   Rozwiązanie lub świadoma akceptacja (np. przeniesienie do backlogu) defektów o średnim i niskim priorytecie.
*   Osiągnięcie zdefiniowanych metryk (np. % wykonanych testów, % testów zakończonych sukcesem).
*   Akceptacja wyników testów przez interesariuszy (np. Product Owner).

**9. Metryki Testowe**

*   Liczba zaplanowanych/wykonanych/zakończonych sukcesem/niepowodzeniem przypadków testowych.
*   Pokrycie wymagań testami.
*   Liczba znalezionych/naprawionych/otwartych defektów (wg priorytetu/krytyczności).
*   Gęstość defektów (liczba defektów na jednostkę kodu/funkcjonalność).
*   Średni czas odpowiedzi API (dla kluczowych endpointów).
*   Pokrycie kodu testami automatycznymi (jeśli monitorowane).

**10. Dostarczane Artefakty**

*   Plan Testów (ten dokument).
*   Przypadki Testowe (w narzędziu do zarządzania testami lub w dokumencie).
*   Skrypty testów automatycznych (w repozytorium kodu).
*   Raporty o Defektach (w systemie śledzenia błędów, np. Jira, GitHub Issues).
*   Raporty z Wykonania Testów (podsumowujące przebieg i wyniki cykli testowych).
*   Raport Końcowy z Testów (podsumowanie całego procesu testowania dla MVP).

**11. Ryzyka i Założenia**

**Ryzyka:**

*   **Zależność od OpenRouter.ai:** Niestabilność API, zmiany w API, limity użycia, koszty, błędy zwracane przez AI.
*   **Zarządzanie Kluczami API:** Ryzyko wycieku kluczy (obecnie w `appsettings.json` - **zalecane użycie np. User Secrets, Azure Key Vault, Environment Variables**).
*   **Niejasne Wymagania:** Szczególnie dotyczące Spaced Repetition i Analityki w MVP.
*   **Środowisko Testowe:** Brak środowiska zbliżonego do produkcyjnego może ukryć pewne problemy. Niespójności między środowiskami.
*   **Dane Testowe:** Trudność w przygotowaniu realistycznych i różnorodnych danych tekstowych do generowania fiszek. Konieczność zarządzania stanem bazy danych między testami.
*   **Złożoność Blazor Server:** Potencjalne trudności w automatyzacji E2E ze względu na SignalR.
*   **Ograniczone Zasoby/Czas:** Może wpłynąć na zakres i głębokość testów.
*   **Zmiany w Zakresie (Scope Creep):** Dodawanie nowych funkcji w trakcie fazy testowania MVP.

**Założenia:**

*   Zespół deweloperski będzie dostarczał testowalne wersje aplikacji.
*   Dostępne będą odpowiednie środowiska testowe i dane.
*   Udostępniony zostanie testowy klucz API do OpenRouter.ai (nieprodukcyjny).
*   Będzie istniał mechanizm zgłaszania i śledzenia defektów.
*   Wymagania dotyczące Spaced Repetition i Analityki zostaną doprecyzowane.

**12. Scenariusze Testowe (Wysokopoziomowe)**

Poniżej przedstawiono główne obszary i przykładowe scenariusze do przetestowania. Szczegółowe przypadki testowe zostaną opracowane na ich podstawie.

**12.1. Autentykacja i Autoryzacja Użytkowników**

*   Rejestracja nowego użytkownika (poprawne dane, istniejący email, nieprawidłowy email, za krótkie hasło).
*   Logowanie użytkownika (poprawne dane, błędne hasło, nieistniejący email).
*   Walidacja tokenu JWT (poprawność, wygaśnięcie, próba dostępu bez tokenu, próba dostępu z nieważnym tokenem).
*   Autoryzacja dostępu do zasobów (np. próba pobrania/edycji/usunięcia fiszek innego użytkownika przez API).
*   Sprawdzenie danych użytkownika w tokenie (`userId` w `ClaimsPrincipal`).

**12.2. Zarządzanie Fiszkami (Manualne - CRUD)**

*   Tworzenie nowej fiszki (przez UI/API, poprawne dane, puste pola, przekroczenie limitu znaków).
*   Wyświetlanie listy fiszek zalogowanego użytkownika.
*   Edycja istniejącej fiszki (zmiana front/back, zapisanie zmian).
*   Usuwanie fiszki (potwierdzenie, weryfikacja usunięcia z listy/DB).
*   Weryfikacja, czy operacje CRUD dotyczą tylko fiszek zalogowanego użytkownika.

**12.3. Generowanie Fiszek z Wykorzystaniem AI**

*   Wysłanie tekstu źródłowego do API `/api/generations` (tekst spełniający min/max długość, tekst za krótki/za długi).
*   Wybór modelu AI (jeśli UI na to pozwala, użycie domyślnego modelu).
*   Poprawne przetworzenie żądania i komunikacja z OpenRouter.ai (symulacja sukcesu).
*   Poprawne sparsowanie odpowiedzi z AI i wyświetlenie sugestii fiszek w UI.
*   Obsługa błędów komunikacji z OpenRouter (timeout, błąd API key, błąd 5xx serwera AI).
*   Obsługa błędów formatu odpowiedzi z AI (np. niepoprawny JSON).
*   Zapis rekordu `Generation` w bazie danych po pomyślnym wygenerowaniu.
*   Zapis rekordu `GenerationErrorLog` w bazie danych w przypadku błędu generowania.
*   Weryfikacja hashowania tekstu źródłowego (`SourceTextHash`).

**12.4. Zapisywanie Wygenerowanych Fiszek (Batch Save)**

*   Wysłanie żądania do API `/api/flashcards/batch` z listą fiszek do zapisania (poprawne dane).
*   Poprawne zapisanie fiszek w bazie danych z powiązaniem do rekordu `Generation`.
*   Weryfikacja odpowiedzi API (lista zapisanych fiszek, podsumowanie).
*   Obsługa pustej listy fiszek do zapisania.
*   Obsługa błędów walidacji danych wejściowych dla poszczególnych fiszek.
*   Sprawdzenie, czy `UserId` jest poprawnie przypisany do zapisywanych fiszek.

**12.5. Testy API (Bezpośrednie)**

*   Sprawdzenie każdego endpointu pod kątem:
    *   Poprawnych metod HTTP (GET, POST, PUT, DELETE).
    *   Oczekiwanych kodów statusu HTTP (200, 201, 204, 400, 401, 403, 404, 500).
    *   Poprawności formatu żądania i odpowiedzi (JSON).
    *   Wymagania nagłówka `Authorization` dla chronionych endpointów.
    *   Obsługi błędów walidacji (np. brakujące pola, niepoprawny format danych).
    *   Poprawności nagłówków CORS (w odpowiedzi na żądania z dozwolonych domen).
    *   Reakcji na nieoczekiwane dane wejściowe.

**12.6. Testy Interfejsu Użytkownika (Frontend - Blazor)**

*   Renderowanie kluczowych komponentów (`AuthTabs`, `FlashcardList`, `GenerowanieFiszekView`, `NavMenu`, etc.).
*   Nawigacja między stronami/widokami.
*   Poprawność działania formularzy (rejestracja, logowanie, edycja fiszki, wklejanie tekstu).
*   Walidacja po stronie klienta (jeśli zaimplementowana).
*   Wyświetlanie komunikatów o sukcesie/błędzie (np. po rejestracji, logowaniu, generowaniu, zapisie).
*   Interakcja z komponentami MudBlazor (przyciski, pola tekstowe, dialogi, tabele).
*   Poprawne wyświetlanie listy fiszek.
*   Działanie mechanizmu przechowywania tokenu (np. LocalStorage) i odnawiania sesji.
*   Obsługa stanu braku autoryzacji (`NotAuthorizedCustom`).

**12.7. Testy Bazy Danych**

*   Weryfikacja schematu bazy danych po zastosowaniu migracji.
*   Sprawdzenie integralności referencyjnej (Foreign Keys, np. User <-> Flashcard, Generation <-> Flashcard).
*   Weryfikacja ograniczeń (Constraints, np. Unique Email, MaxLength).
*   Sprawdzenie automatycznego ustawiania pól `CreatedAt` i `UpdatedAt` przy tworzeniu i modyfikacji encji.
*   Testowanie działania `DeleteBehavior.Cascade` i `DeleteBehavior.SetNull` zgodnie z definicją w `DbContext`.

**12.8. Testy Bezpieczeństwa**

*   Skanowanie pod kątem podstawowych podatności (np. za pomocą narzędzi OWASP ZAP - na środowisku testowym).
*   Próby obejścia autoryzacji (dostęp do cudzych danych).
*   Weryfikacja polityki haseł (jeśli zdefiniowana poza minimalną długością).
*   Sprawdzenie, czy wrażliwe informacje (klucze API, hasła) nie są przesyłane/logowane w sposób niezaszyfrowany.
*   Testowanie walidacji wejściowej pod kątem potencjalnych wektorów ataku (SQLi, XSS).
*   Weryfikacja konfiguracji JWT (czas życia, siła klucza - min. 16 znaków to mało, zalecane dłuższe, losowe klucze!).
*   Użycie HTTPS.

**12.9. Testy Wydajności**

*   Pomiar czasu odpowiedzi endpointu `/api/generations` dla różnych długości tekstu źródłowego.
*   Pomiar czasu odpowiedzi endpointu `/api/flashcards/batch` dla różnej liczby zapisywanych fiszek.
*   Pomiar czasu odpowiedzi endpointu pobierającego listę fiszek (`/api/flashcards`) dla użytkownika z dużą liczbą fiszek.
*   Monitorowanie zużycia zasobów (CPU, pamięć) serwera API pod obciążeniem (jeśli możliwe).

**12.10. Testy Konfiguracji**

*   Uruchomienie API z poprawną konfiguracją.
*   Uruchomienie API z brakującym/niepoprawnym kluczem OpenRouter (sprawdzenie `OpenRouterOptionsValidator`).
*   Uruchomienie API z niepoprawnym Connection String.
*   Sprawdzenie działania aplikacji przy różnych ustawieniach JWT (np. krótki czas wygaśnięcia).

**12.11. (MVP) Integracja z Systemem Powtórek (Spaced Repetition)**

*   (Wymaga doprecyzowania implementacji MVP) Testowanie podstawowego mechanizmu planowania/wyświetlania fiszek do powtórki.

**12.12. (MVP) Analityka**

*   (Wymaga doprecyzowania implementacji MVP) Weryfikacja, czy statystyki są poprawnie zliczane i zapisywane/wyświetlane.

**13. Narzędzia**

*   **Przeglądarki:** Najnowsze wersje Chrome, Firefox, Edge.
*   **Narzędzia Deweloperskie Przeglądarki:** Do inspekcji DOM, sieci, konsoli.
*   **API Client:** Postman, Insomnia (do manualnych i pół-automatycznych testów API).
*   **Testy Automatyczne API:** Biblioteka `HttpClient` w C#, frameworki jak RestSharp lub dedykowane narzędzia.
*   **Testy Automatyczne E2E:** Playwright, Selenium (z odpowiednimi sterownikami dla Blazor).
*   **Testy Jednostkowe/Integracyjne:** xUnit, Moq (zgodnie z projektem API).
*   **Baza Danych:** SQL Server Management Studio (SSMS), Azure Data Studio (do inspekcji danych i schematu).
*   **System Śledzenia Błędów:** GitHub Issues, Jira, Azure DevOps Boards.
*   **Narzędzia do Testów Wydajności:** k6, Apache JMeter (opcjonalnie dla podstawowych testów).
*   **Narzędzia do Testów Bezpieczeństwa:** OWASP ZAP (opcjonalnie dla podstawowego skanowania).
*   **Zarządzanie Testami:** TestRail, Zephyr, Xray (jeśli dostępne) lub arkusze kalkulacyjne/dokumenty tekstowe.

**14. Zarządzanie Defektami**

*   Wszystkie znalezione defekty będą raportowane w dedykowanym systemie śledzenia błędów.
*   Każdy raport o defekcie powinien zawierać:
    *   Tytuł (jasny i zwięzły).
    *   Opis (kroki do reprodukcji, co się stało, co powinno się stać).
    *   Środowisko testowe.
    *   Wersja aplikacji/build.
    *   Priorytet (np. Krytyczny, Wysoki, Średni, Niski).
    *   Krytyczność (np. Bloker, Poważny, Drobny, Kosmetyczny).
    *   Zrzuty ekranu/logi (jeśli to możliwe).
*   Cykl życia defektu: Zgłoszony -> W Analizie -> Przypisany -> W Naprawie -> Gotowy do Testów -> Testowany -> Zamknięty / Ponownie Otwarty.
*   Regularne spotkania (np. Bug Triage) w celu omówienia i priorytetyzacji zgłoszonych defektów.

---

Ten plan testów stanowi podstawę do przeprowadzenia kompleksowych działań zapewnienia jakości dla projektu "10x-cards" w zakresie MVP. Będzie on aktualizowany w miarę postępu prac i pojawiania się nowych informacji lub zmian w wymaganiach.