# Szczegółowa Specyfikacja Modułu Autentykacji

## I. ARCHITEKTURA INTERFEJSU UŻYTKOWNIKA

### A. Strony i Layouty
1. Strony dostępne dla użytkowników niezalogowanych:
   - **Login:** Formularz logowania zawierający pola `Email` i `Hasło`.
   - **Rejestracja:** Formularz do tworzenia nowego konta z polami `Email`, `Hasło` oraz `Potwierdzenie hasła`.
   - **Odzyskiwanie hasła:** Formularz umożliwiający wysyłkę linku resetującego hasło.
   - Wszystkie strony nieautoryzowane korzystają z dedykowanego layoutu, np. `PublicLayout.razor`.
2. Strony dostępne dla zalogowanych użytkowników:
   - **Dashboard:** Główny widok z informacjami użytkownika.
   - **Zarządzanie fiszkami:** Widok listy fiszek z możliwością edycji, usuwania i innych akcji.
   - Wykorzystanie layoutu autoryzowanego, np. `MainLayoutAuth.razor`, zawierającego pasek nawigacyjny, informację o użytkowniku oraz przycisk wylogowania.

### B. Komponenty i ich odpowiedzialności
1. **Formularz Logowania**
   - Pola: `Email`, `Hasło`.
   - Walidacja: Sprawdzenie formatu email, wymagane pola.
   - Komunikaty błędów: Błędne dane logowania, brak wypełnionych pól.
2. **Formularz Rejestracji**
   - Pola: `Email`, `Hasło`, `Potwierdzenie hasła`.
   - Walidacja: Spójność haseł, poprawny format email, minimalna długość hasła.
   - Komunikaty błędów: Użytkownik już istnieje, niepoprawne dane.
3. **Formularz Odzyskiwania Hasła**
   - Pola: `Email`.
   - Opis działania: Weryfikacja istnienia adresu email i wysyłka linku resetującego.
   - Walidacja: Sprawdzenie istnienia adresu email w bazie.
4. **Komponent Nawigacji**
   - Dynamiczne renderowanie elementów w zależności od stanu autentykacji.
   - Wyświetlanie informacji o aktualnie zalogowanym użytkowniku oraz przycisku wylogowania.
5. **Komponenty Powiadomień**
   - Użycie komponentów takich jak `<MudSnackbar>` z biblioteki MudBlazor do prezentacji komunikatów o sukcesie i błędach.

### C. Obsługa głównych scenariuszy
1. Poprawne logowanie: Przekierowanie do Dashboard.
2. Błędne dane logowania: Wyświetlenie natychmiastowego komunikatu o błędzie.
3. Rejestracja:
   - Sukces: Utworzenie nowego konta oraz ewentualne automatyczne zalogowanie lub potwierdzenie rejestracji.
   - Błąd: Wyświetlenie informacji o błędzie wysłanych przez backend.
4. Odzyskiwanie hasła:
   - Wysłanie linku resetującego hasło.
   - Powiadomienie użytkownika o wysłaniu instrukcji resetowania.

## II. LOGIKA BACKENDOWA

### A. Struktura API i modele danych
1. **Endpointy API:**
   - `POST /api/auth/register` – Rejestracja użytkownika.
   - `POST /api/auth/login` – Logowanie użytkownika.
   - `POST /api/auth/reset` – Inicjacja procesu odzyskiwania/resetowania hasła.
   - `POST /api/auth/logout` – Wylogowanie użytkownika.
2. **Modele danych (DTO):**
   - `RegisterModel`: Zawiera pola `Email`, `Hasło`, `PotwierdzenieHasła`.
   - `LoginModel`: Zawiera pola `Email`, `Hasło`.
   - `ResetPasswordModel`: Zawiera pola `Email`, `Token`, `NoweHasło`, `PotwierdzenieNowegoHasła`.
3. **Mechanizm walidacji:**
   - Wykorzystanie atrybutów DataAnnotations (np. `[Required]`, `[EmailAddress]`, `[Compare]`).
   - Globalny middleware do przetwarzania i raportowania błędów walidacyjnych.

### B. Obsługa wyjątków i komunikacja
- Globalny handler błędów zwracający odpowiednie kody HTTP oraz komunikaty błędów.
- Rejestrowanie wyjątków w systemie logów.
- Dynamiczna aktualizacja renderowania stron w zależności od stanu autoryzacji.

## III. SYSTEM AUTENTYKACJI

### A. Wykorzystanie ASP.NET Core Identity
- Konfiguracja w `Program.cs`:
  - `builder.Services.AddIdentity<ApplicationUser, IdentityRole>()...`
  - Użycie EF Core do komunikacji z bazą MS SQL Server, gdzie przechowywane są dane użytkowników.
- Implementacja mechanizmu uwierzytelnienia opartego na cookie.

### B. Zarządzanie kontami użytkowników
- CRUD dla użytkowników, zarządzanie rolami (np. `User`, `Admin`).
- Funkcjonalności resetowania hasła oraz weryfikacji konta.
- Dodatkowe zabezpieczenia, np. lockout po kilku nieudanych próbach logowania.

### C. Integracja z logiką biznesową
- Ochrona endpointów API przy użyciu atrybutów i middleware autoryzacji.
- Spójne zarządzanie sesjami i tokenami autoryzacyjnymi.

### D. Zabezpieczenia i standardy
- Ochrona przed atakami CSRF i SQL Injection, wykorzystując domyślne mechanizmy ASP.NET Core.
- Szyfrowanie haseł oraz bezpieczne przechowywanie danych.

## Kluczowe Wnioski:
- Rozdzielenie warstwy UI na sekcje dla użytkowników niezalogowanych i zalogowanych zapewnia przejrzystość i bezpieczeństwo.
- Oba formularze (logowanie, rejestracja, odzyskiwanie hasła) posiadają dedykowane mechanizmy walidacji po stronie klienta i serwera.
- Backend oparty jest o REST API, korzystające z ASP.NET Core Identity oraz EF Core, co umożliwia skalowalność i bezpieczeństwo.
- Cały moduł jest ściśle zintegrowany z istniejącym systemem, zapewniając spójność z resztą aplikacji oraz spełniając wymagania wyznaczone w dokumentacji `prd.md` oraz stacku określonym w `tech-stack.md`.
