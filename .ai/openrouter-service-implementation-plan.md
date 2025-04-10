# Plan wdrożenia usługi OpenRouter

## 1. Opis usługi
Usługa OpenRouter integruje API OpenRouter w celu uzupełnienia czatów opartych na LLM. Jej głównym zadaniem jest wysyłanie precyzyjnie sformułowanych zapytań do modelu AI, a następnie odbieranie i analizowanie ustrukturyzowanych odpowiedzi. Usługa korzysta z następujących elementów:

1. **Komunikacja HTTP** – realizacja zapytań do API OpenRouter z wykorzystaniem HttpClient w ASP.NET Core.
2. **Budowa komunikatów** – tworzenie komunikatów systemowych i użytkownika oraz formułowanie payloadu z odpowiednimi parametrami modelu.
3. **Parsowanie odpowiedzi** – analizowanie odpowiedzi od API, ze szczególnym uwzględnieniem response_format, który określa strukturę JSON.
4. **Obsługa błędów** – centralizacja logiki wykrywania, rejestrowania oraz radzenia sobie z błędami komunikacyjnymi i walidacyjnymi.

## 2. Opis konstruktora
Konstruktor usługi odpowiedzialny jest za inicjalizację niezbędnych komponentów, takich jak HttpClient, konfiguracja API (adresy, klucze, timeouty) oraz parametry modelu. W konstruktorze można również ustawić domyślne komunikaty systemowe, które będą modyfikowane w trakcie akcencji np. w zależności od kontekstu.

## 3. Publiczne metody i pola
**Publiczne metody:**
1. `SendRequest` – metoda wysyłająca zapytanie do API OpenRouter, przyjmująca komunikaty od użytkownika oraz dodatkowe parametry.
2. `SetModelParameters` – metoda umożliwiająca konfigurację parametrów modelu (np. temperatura, top_p, max_tokens).
3. `ParseResponse` – metoda przetwarzająca odpowiedź API zgodnie z zadeklarowanym response_format.
4. `LogCommunication` – metoda logująca szczegóły komunikacji (do debugowania i audytu).

**Publiczne pola:**
- `ApiEndpoint` – URL endpointu API OpenRouter.
- `DefaultModelName` – domyślna nazwa modelu (np. "openrouter-llm-v1").
- `DefaultParameters` – domyślne parametry modelu, np. { temperature: 0.7, top_p: 0.95, max_tokens: 150 }.

## 4. Prywatne metody i pola
**Prywatne metody:**
1. `BuildPayload` – buduje strukturę zapytania ze wszystkimi wymaganymi parametrami, takimi jak komunikat systemowy, komunikat użytkownika, response_format, nazwa modelu i parametry modelu.
2. `ValidateResponse` – waliduje strukturę odpowiedzi otrzymanej z API, sprawdzając zgodność z zadeklarowanym schematem JSON.
3. `HandleApiError` – zarządza błędami po stronie API, implementując strategie retry oraz fallback.

**Prywatne pola:**
- `httpClient` – instancja HttpClient używana do komunikacji z API.
- `internalLogger` – mechanizm logowania błędów i zdarzeń systemowych.

## 5. Obsługa błędów
W usłudze należy przewidzieć następujące scenariusze błędów:
1. **Błąd połączenia z API:**
   - *Wyzwanie:* API może być niedostępne lub wystąpić timeout.
   - *Rozwiązanie:* Implementacja retry logic, timeout setting oraz mechanizmu circuit breaker.
2. **Błąd walidacji odpowiedzi:**
   - *Wyzwanie:* Odpowiedź może nie zgadzać się z zadeklarowanym schematem JSON.
   - *Rozwiązanie:* Walidacja odpowiedzi za pomocą schematu JSON, logowanie błędów walidacyjnych i zwracanie kompensacyjnej informacji.
3. **Błąd autoryzacji:**
   - *Wyzwanie:* Błędny klucz API lub brak uprawnień.
   - *Rozwiązanie:* Weryfikacja klucza przed wysłaniem zapytania, obsługa błędów 401/403.
4. **Błąd wejścia:**
   - *Wyzwanie:* Niepoprawne dane wejściowe użytkownika.
   - *Rozwiązanie:* Walidacja danych wejściowych oraz informowanie użytkownika o błędach przy użyciu czytelnych komunikatów.

## 6. Kwestie bezpieczeństwa
- **Przechowywanie i użycie kluczy API:** Przechowywanie w bezpiecznych magazynach (np. Azure Key Vault, AWS Secrets Manager) oraz użycie zmiennych środowiskowych.
- **Szyfrowanie komunikacji:** Wymuszenie HTTPS dla wszystkich połączeń do API.
- **Ochrona przed atakami:** Implementacja rate limiting, walidacja danych wejściowych oraz ochrona przed atakami typu injection.
- **Audyt i logowanie:** Monitorowanie wszystkich operacji komunikacyjnych oraz implementacja systemu alertów przy wykryciu nieautoryzowanych prób dostępu.

## 7. Plan wdrożenia krok po kroku
1. **Przygotowanie środowiska:**
   - Skonfigurowanie projektu ASP.NET Core oraz dodanie niezbędnych pakietów NuGet (HttpClient, biblioteki logujące, itp.).
   - Ustawienie zmiennych środowiskowych dla kluczy i endpointów API.
2. **Implementacja konstruktora usługi:**
   - Inicjalizacja HttpClient oraz ustawienie domyślnych parametrów (DefaultModelName, DefaultParameters).
3. **Budowa komponentu komunikacji:**
   - Zaimplementowanie metody `BuildPayload` z uwzględnieniem poniższych elementów:
     1. **Komunikat systemowy:**
        - Przykład: "System: Udziel precyzyjnej i wyczerpującej odpowiedzi, uwzględniając aktualny kontekst rozmowy.".
     2. **Komunikat użytkownika:**
        - Przykład: "Użytkownik: Proszę opisać szczegóły procesu wdrożenia.".
     3. **Response_format:**
        - Przykład: 
          ```json
          { "type": "json_schema", "json_schema": { "name": "chatResponse", "strict": true, "schema": { "answer": { "type": "string" }, "metadata": { "type": "object" } } } }
          ```
     4. **Nazwa modelu:**
        - Przykład: "openrouter-llm-v1".
     5. **Parametry modelu:**
        - Przykład: { "temperature": 0.7, "top_p": 0.95, "max_tokens": 150 }.
4. **Implementacja metody wysyłania zapytań:**
   - Zaimplementowanie metody `SendRequest`, która wywołuje API OpenRouter i przekazuje zbudowany payload.
5. **Parsowanie i walidacja odpowiedzi:**
   - Implementacja metody `ParseResponse` z dodatkową metodą `ValidateResponse` do sprawdzania zgodności odpowiedzi z zadeklarowanym schematem.
6. **Obsługa błędów i logowanie:**
   - Integracja metod `HandleApiError` oraz `LogCommunication` w celu zarządzania wyjątkami i rejestrowania komunikacji.
7. **Testy integracyjne i jednostkowe:**
   - Przeprowadzenie testów dla kluczowych scenariuszy, w tym symulowania błędów połączenia, niepoprawnych odpowiedzi oraz walidacji poprawności payloadu.
8. **Wdrożenie i monitorowanie:**
   - Wdrożenie usługi na środowisko deweloperskie, następnie testy w środowisku produkcyjnym.
   - Implementacja mechanizmów monitoringu oraz alertów w celu bieżącego śledzenia stanu usługi.

---

Powyższy przewodnik stanowi kompleksowy plan wdrożenia usługi OpenRouter, który powinien być łatwo adaptowany do specyfiki stosu technologicznego wykorzystywanego w projekcie 1xCards. 