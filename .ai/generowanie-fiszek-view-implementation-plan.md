# Plan implementacji widoku Generowanie fiszek

## 1. Przegląd
Widok "Generowanie fiszek" umożliwia zalogowanemu użytkownikowi wklejenie tekstu (od 1000 do 10 000 znaków) oraz wygenerowanie na jego podstawie propozycji fiszek przy użyciu zewnętrznego API AI. Po wygenerowaniu fiszek, użytkownik może przeglądać wyniki, zatwierdzać, edytować lub odrzucać poszczególne fiszki. Widok integruje również elementy tworzenia fiszek ręcznie.

## 2. Routing widoku
Widok będzie dostępny pod adresem: `/generowanie-fiszek`.

## 3. Struktura komponentów
- **GenerowanieFiszekView** - główny komponent widoku
  - **TextInputComponent** - pole tekstowe dla wklejania tekstu
  - **GenerateButton** - przycisk inicjujący proces generowania
  - **Loader** - wskaźnik ładowania podczas komunikacji z API
  - **FlashcardList** - lista wyświetlająca wygenerowane fiszki
    - **FlashcardItem** - pojedynczy element listy z fiszką
      - Akcje: zatwierdzenie, edycja, odrzucenie
  - **FlashcardEditDialog** - modal umożliwiający edycję treści fiszki
  - **ManualFlashcardForm** - formularz do ręcznego tworzenia fiszek (opcjonalnie)

## 4. Szczegóły komponentów
### GenerowanieFiszekView
- **Opis**: Główny kontener widoku odpowiedzialny za obsługę wejścia użytkownika, integrację z API i zarządzanie stanem wygenerowanych fiszek.
- **Główne elementy**: pole tekstowe, przycisk generowania, loader, lista fiszek.
- **Obsługiwane interakcje**:
  - Wklejenie tekstu do pola.
  - Kliknięcie przycisku "Generuj fiszki".
  - Wyświetlenie loadera podczas oczekiwania na odpowiedź API.
  - Przeglądanie listy wygenerowanych fiszek.
- **Warunki walidacji**: Tekst musi mieć od 1000 do 10 000 znaków. Walidacja po stronie klienta przed wysłaniem żądania.
- **Typy**: GenerationRequestDTO, GenerationResponseDTO, FlashcardDTO, FlashcardViewModel.
- **Propsy**: Brak (komponent główny zarządza własnym stanem).

### FlashcardList
- **Opis**: Wyświetla listę fiszek otrzymanych z API.
- **Główne elementy**: Iteracja po liście fiszek, prezentacja komponentów FlashcardItem.
- **Obsługiwane interakcje**: Kliknięcia przycisków w poszczególnych fiszkach.
- **Warunki walidacji**: Sprawdzenie, czy lista nie jest pusta przed renderowaniem.
- **Typy**: List<FlashcardViewModel>.
- **Propsy**: Lista fiszek.

### FlashcardItem
- **Opis**: Prezentuje pojedynczą fiszkę wraz z przyciskami akcji (zatwierdź, edytuj, odrzuć).
- **Główne elementy**: Tekst przodu i tyłu, przyciski akcji.
- **Obsługiwane interakcje**:
  - Zatwierdzenie fiszki – oznaczenie fiszki jako wybranej do zapisania.
  - Edycja – otwarcie modala FlashcardEditDialog z danymi fiszki.
  - Odrzucenie – usunięcie fiszki z listy.
- **Warunki walidacji**: Brak dodatkowej walidacji w komponencie, dane pochodzą z backendu.
- **Typy**: FlashcardViewModel.
- **Propsy**: Obiekt fiszki, callbacki dla akcji (onAccept, onEdit, onReject).

### FlashcardEditDialog
- **Opis**: Modal umożliwiający edycję treści fiszki.
- **Główne elementy**: Formularz z polami edycji (przód, tył), przyciski zapisu i anulowania.
- **Obsługiwane interakcje**:
  - Zmiana treści fiszki.
  - Zapis edycji przez kliknięcie przycisku.
- **Warunki walidacji**: Maksymalna długość tekstu (przód do 200 znaków, tył do 500 znaków).
- **Typy**: FlashcardViewModel (używany do edycji), wewnętrzny model formularza.
- **Propsy**: Obiekt fiszki do edycji, callback onSave, callback onCancel.

### ManualFlashcardForm (opcjonalnie)
- **Opis**: Formularz do ręcznego tworzenia nowej fiszki.
- **Główne elementy**: Pola tekstowe dla przodu i tyłu, przycisk zapisu.
- **Obsługiwane interakcje**: Wprowadzanie danych, walidacja i wysyłka formularza.
- **Warunki walidacji**: Maksymalna długość tekstu (przód 200, tył 500).
- **Typy**: FlashcardDTO.
- **Propsy**: Callback onCreate.

## 5. Typy
- **GenerationRequestDTO**: 
  - source_text: string
  - model?: string
- **GenerationResponseDTO**:
  - id: number
  - user_id: number
  - model: string
  - generated_count: number
  - flashcards: FlashcardDTO[]
  - created_at: Date
- **FlashcardDTO**:
  - front: string
  - back: string
  - source: string
- **FlashcardViewModel** (rozszerzenie FlashcardDTO):
  - accepted?: boolean
  - edited?: boolean
  - temporaryId?: string

## 6. Zarządzanie stanem
Stan widoku będzie zarządzany lokalnie w komponencie `GenerowanieFiszekView`:
- Przechowywany tekst wejściowy
- Flaga ładowania (loading)
- Lista fiszek (lista obiektów typu FlashcardViewModel)
- Komunikaty błędów (error message)

Ewentualnie można stworzyć niestandardowy hook lub usługę do obsługi wywołań API oraz przechowywania stanu, jednak w Blazor Server wystarczy użycie lokalnego stanu komponentu.

## 7. Integracja API
Integracja odbywa się poprzez wywołanie endpointu **POST /generations**:
- **Żądanie**: Obiekt GenerationRequestDTO zawierający `source_text` oraz opcjonalnie `model`.
- **Odpowiedź**: Obiekt GenerationResponseDTO, z listą wygenerowanych fiszek.

Wywołanie odbywa się asynchronicznie przy użyciu `HttpClient` wstrzykniętego poprzez dependency injection. W przypadku błędów (np. 400, 500, 401) należy wyświetlić stosowny komunikat użytkownikowi.

## 8. Interakcje użytkownika
- Użytkownik wkleja tekst do pola tekstowego.
- Po kliknięciu przycisku "Generuj fiszki":
  - Rozpoczyna się walidacja długości tekstu.
  - Wyświetlany jest loader.
  - Następuje wywołanie API. 
  - Po otrzymaniu odpowiedzi, lista fiszek zostaje wyświetlona.
- Dla każdej fiszki:
  - Kliknięcie przycisku "Zatwierdź" oznacza wybór fiszki do dalszego zapisu.
  - Kliknięcie przycisku "Edytuj" otwiera modal umożliwiający zmianę treści.
  - Kliknięcie przycisku "Odrzuć" usuwa fiszkę z listy.
- Opcjonalnie: Użytkownik może ręcznie dodać nową fiszkę przy użyciu formularza.

## 9. Warunki i walidacja
- Walidacja długości tekstu wejściowego: minimum 1000, maksimum 10 000 znaków.
- Podczas edycji fiszki: ograniczenie do 200 znaków dla przodu i 500 znaków dla tyłu.
- Przed wysłaniem danych do API sprawdzane są poprawności pól (wymagalność, długość).
- Dostęp do widoku mają tylko uwierzytelnieni użytkownicy.

## 10. Obsługa błędów
- Wyświetlanie komunikatu przy nieprawidłowych danych wejściowych (np. za krótki tekst).
- Obsługa błędów po stronie API: komunikaty dla błędów 400, 500 oraz 401.
- Użycie komponentu powiadomień (np. Snackbar z MudBlazor) do prezentacji błędów użytkownikowi.

## 11. Kroki implementacji
1. Utworzenie nowego komponentu `GenerowanieFiszekView.razor` i konfiguracja routingu na `/generowanie-fiszek`.
2. Implementacja pola tekstowego i przycisku generowania przy użyciu komponentów MudBlazor (np. MudTextField, MudButton).
3. Dodanie logiki walidacji długości tekstu wewnątrz komponentu.
4. Konfiguracja wywołania API poprzez `HttpClient`, obsługa stanu ładowania i komunikatów błędów.
5. Po otrzymaniu odpowiedzi API, przetworzenie danych i zapisanie listy fiszek w stanie komponentu.
6. Implementacja komponentu `FlashcardList` wraz z komponentem `FlashcardItem` do wyświetlania każdej fiszki.
7. Dodanie obsługi akcji dla poszczególnych fiszek (zatwierdzenie, edycja, odrzucenie) oraz integracja z modalem `FlashcardEditDialog`.
8. Opcjonalnie: Utworzenie formularza `ManualFlashcardForm` do ręcznego dodawania fiszek.
9. Testowanie widoku, walidacja interakcji użytkownika oraz obsługa błędów.
10. Integracja komponentu z resztą aplikacji i finalne testy w środowisku developerskim. 