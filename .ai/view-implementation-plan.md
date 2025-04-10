# API Endpoint Implementation Plan: POST /generations

## 1. Przegląd punktu końcowego
Endpoint POST /generations służy do generowania sugestii fiszek przy użyciu zewnętrznego modelu AI (np. Openrouter.ai). Jego główne zadania to:
- Walidacja danych wejściowych zgodnie z regułami (np. długość `source_text` musi wynosić od 1000 do 10000 znaków).
- Wywołanie asynchroniczne zewnętrznego API AI z użyciem HttpClient.
- Zapisanie szczegółów generacji do tabeli `Generations` oraz wygenerowanych fiszek do tabeli `Flashcards`.
- Logowanie błędów w przypadku niepowodzenia wywołania API do tabeli `Generation_Error_Logs`.

## 2. Szczegóły żądania
- **Metoda HTTP:** POST
- **Struktura URL:** /generations
- **Parametry:**
  - **Wymagane w treści żądania:**
    - `source_text`: string (długość od 1000 do 10000 znaków)
  - **Opcjonalne w treści żądania:**
    - `model`: string (jeśli nie zostanie podany, używany jest model domyślny)
- **Nagłówki:** JWT Bearer token (wymagany do uwierzytelnienia)
- **Przykładowa struktura JSON:**
  ```json
  {
    "source_text": "...",
    "model": "opcjonalny_model"
  }
  ```

## 3. Wykorzystywane typy
- **DTO oraz Command Modele:**
  - `GenerationRequestDTO`
    - `string source_text`
    - `string? model`
  - `GenerationResponseDTO`
    - `int id`
    - `int user_id`
    - `string model`
    - `int generated_count`
    - `List<GenerationFlashcardDTO> flashcards`
    - `DateTime created_at`
  - `GenerationFlashcardDTO`
    - `string front`
    - `string back`
    - `string source` (oczekiwane wartości: "ai-full" lub "ai-edited")

## 4. Szczegóły odpowiedzi
- **Kody statusu:** 
  - 201 Created – operacja przebiegła pomyślnie
  - 400 Bad Request – nieprawidłowe dane wejściowe (np. nieodpowiednia długość `source_text`)
  - 401 Unauthorized – brak lub nieprawidłowy token JWT
  - 500 Internal Server Error – błąd przy wywołaniu zewnętrznego API lub inny krytyczny błąd serwera
- **Przykładowa struktura odpowiedzi:**
  ```json
  {
    "id": 123,
    "user_id": 45,
    "model": "nazwa_modelu",
    "generated_count": 5,
    "flashcards": [
      { "front": "...", "back": "...", "source": "ai-full" }
    ],
    "created_at": "2023-10-10T12:00:00Z"
  }
  ```

## 5. Przepływ danych
1. Klient wysyła żądanie POST /generations z polem `source_text` oraz opcjonalnym `model`.
2. Middleware walidacyjny oraz atrybuty (np. DataAnnotations) weryfikują poprawność danych wejściowych.
3. Endpoint przekazuje dane do warstwy serwisowej, czyli `GenerationService`.
4. W `GenerationService`:
   - Dokonywana jest dodatkowa walidacja (np. długość `source_text`).
   - Wywoływane jest asynchroniczne zewnętrzne API AI przy użyciu HttpClient oraz obsługa timeoutów.
   - W przypadku powodzenia, zapisywane są dane generacji w tabeli `Generations` oraz tworzone wpisy dla wygenerowanych fiszek w tabeli `Flashcards`.
   - W przypadku niepowodzenia, szczegóły błędu (model, skrót `source_text_hash`, komunikat błędu) są logowane w tabeli `Generation_Error_Logs`.
5. Endpoint zwraca odpowiedź 201 Created z danymi dotyczącymi generacji lub odpowiedni kod błędu.

## 6. Względy bezpieczeństwa
- **Uwierzytelnianie i Autoryzacja:** Endpoint zabezpieczony mechanizmem JWT Bearer token; dostęp mają tylko uwierzytelnieni użytkownicy.
- **Walidacja danych:** Stosowanie atrybutów walidacyjnych oraz dodatkowych reguł w serwisie, aby uniemożliwić ataki, takie jak SQL Injection lub próby przekroczenia maksymalnych długości pól.
- **Bezpieczne logowanie błędów:** Szczegóły błędów nie są ujawniane użytkownikowi; pełne informacje zapisywane są jedynie w zabezpieczonej tabeli `Generation_Error_Logs`.

## 7. Obsługa błędów
- **400 Bad Request:** Zwracany, gdy walidacja wejściowa (np. długość `source_text`) nie spełnia wymagań.
- **401 Unauthorized:** Gdy token JWT jest nieobecny lub nieważny.
- **500 Internal Server Error:** W przypadku błędu podczas wywołania zewnętrznego API lub innych krytycznych błędów; wszystkie wyjątki są przechwytywane, a szczegóły błędu logowane w tabeli `Generation_Error_Logs`.

## 8. Rozważania dotyczące wydajności
- Asynchroniczne wywołanie zewnętrznego API w celu redukcji opóźnień.
- Ustawienie timeoutów dla HttpClient, aby zabezpieczyć się przed zawieszeniem się operacji.
- Optymalizacja operacji bazy danych (np. zastosowanie eager loading, jeżeli jest to konieczne) dla szybszego zapisu i odczytu.
- Możliwość cache'owania wyników w przypadku powtarzających się similar requestów, jeśli jest to możliwe w danym scenariuszu.

## 9. Etapy wdrożenia
1. **Definicja modeli:** Utworzenie DTO (`GenerationRequestDTO`, `GenerationResponseDTO`, `GenerationFlashcardDTO`) oraz modeli komend, jeśli będą potrzebne.
2. **Implementacja warstwy serwisowej:** Opracowanie `GenerationService`, która będzie zawierać logikę walidacji, wywołania zewnętrznego API oraz logowania błędów.
3. **Konfiguracja Minimal API:** Dodanie endpointu w pliku startowym aplikacji (np. `Program.cs`) przy użyciu `app.MapPost("/generations", ...)`.
4. **Walidacja danych:** Integracja walidacji przy użyciu atrybutów DataAnnotations i/lub dodatkowych reguł walidacyjnych w warstwie serwisowej.
5. **Integracja z EF Core:** Realizacja operacji zapisu do tabel `Generations`, `Flashcards` oraz `Generation_Error_Logs`.
6. **Obsługa wyjątków:** Dodanie globalnego middleware do przechwytywania błędów i konwersji wyjątków na odpowiednie kody statusu HTTP.
7. **Testowanie:** Implementacja testów jednostkowych i integracyjnych, aby upewnić się, że wszystkie ścieżki przepływu (sukces, walidacja, błąd API) działają poprawnie.
8. **Code Review i wdrożenie:** Przeprowadzenie przeglądu kodu, wdrożenie na środowisko testowe oraz monitorowanie logów i wyników wydajnościowych przed produkcyjnym wdrożeniem. 