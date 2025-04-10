# Architektura UI dla 10x-cards

## 1. Przegląd struktury UI

Architektura interfejsu użytkownika dla 10x-cards opiera się na nowoczesnych zasadach projektowania z wykorzystaniem MudBlazor. Pozwala na intuicyjną nawigację, responsywność oraz wysoką dostępność i bezpieczeństwo. Kluczowym elementem systemu jest centralne zarządzanie stanem aplikacji, obejmujące m.in. przechowywanie tokenu JWT i statusu sesji nauki. Strukturę UI dzielimy na kilka głównych widoków odpowiadających kluczowym funkcjom aplikacji, zgodnych z PRD, planem API oraz ustaleniami sesji planowania.

## 2. Lista widoków

### Widok autoryzacji

- **Nazwa widoku:** Autoryzacja
- **Ścieżka widoku:** /auth
- **Główny cel:** Umożliwienie logowania i rejestracji użytkownika oraz zarządzanie sesją (JWT).
- **Kluczowe informacje do wyświetlenia:** Formularze logowania i rejestracji, komunikaty walidacyjne, linki do odzyskiwania hasła.
- **Kluczowe komponenty widoku:** Formularze (inputy, przyciski), walidacja inline, mechanizm snackbar do komunikatów o błędach i sukcesach.
- **UX, dostępność i względy bezpieczeństwa:** Dostępny dla osób z niepełnosprawnościami, zabezpieczenia przed atakami typu brute force, szyfrowanie danych wejściowych.

### Dashboard

- **Nazwa widoku:** Dashboard
- **Ścieżka widoku:** /dashboard
- **Główny cel:** Prezentacja podsumowania stanu fiszek, statystyk oraz szybkiego dostępu do innych modułów.
- **Kluczowe informacje do wyświetlenia:** Liczba fiszek, ostatnie akcje, alerty, podsumowanie sesji nauki.
- **Kluczowe komponenty widoku:** Karty statystyk, wykresy, przyciski nawigacyjne, dynamicznie aktualizowane widgety.
- **UX, dostępność i względy bezpieczeństwa:** Intuicyjny układ, przejrzysta hierarchia informacji, responsive design.

### Widok generowania fiszek

- **Nazwa widoku:** Generowanie fiszek 
- **Ścieżka widoku:** /generowanie
- **Główny cel:** Umożliwienie użytkownikowi wprowadzenia tekstu i uzyskania propozycji fiszek generowanych przez AI.
- **Kluczowe informacje do wyświetlenia:** Pole tekstowe do wprowadzania danych, lista wygenerowanych fiszek, opcje akceptacji, edycji i odrzucenia każdej propozycji.
- **Kluczowe komponenty widoku:** Pole tekstowe z limitem znaków, lista elementów z przyciskami akcji, opcja zbiorczego zapisu („Zapisz wszystkie”/„Zapisz zatwierdzone”), snackbar informujący o wynikach operacji.
- **UX, dostępność i względy bezpieczeństwa:** Walidacja długości tekstu, asynchroniczne przetwarzanie danych, jasne komunikaty o błędach oraz ochrona przed nieautoryzowanym dostępem.

### Widok listy fiszek

- **Nazwa widoku:** Lista fiszek
- **Ścieżka widoku:** /fiszki
- **Główny cel:** Przegląd, edycja oraz usuwanie istniejących fiszek użytkownika.
- **Kluczowe informacje do wyświetlenia:** Lista fiszek (przód i tył), informacja o źródle (manual, ai-full, ai-edited), daty utworzenia/aktualizacji.
- **Kluczowe komponenty widoku:** Dynamicznie aktualizowana tabela lub lista, modal do edycji fiszki, mechanizm potwierdzania usunięcia.
- **UX, dostępność i względy bezpieczeństwa:** Aktualizacje w czasie rzeczywistym, intuicyjny interfejs edycji, potwierdzenia operacji krytycznych, dostępność klawiaturowa.

### Panel użytkownika (Profil)

- **Nazwa widoku:** Panel użytkownika
- **Ścieżka widoku:** /konto
- **Główny cel:** Prezentacja historii aktywności, ustawień konta oraz opcji związanych z zarządzaniem profilem.
- **Kluczowe informacje do wyświetlenia:** Historia działań, ustawienia konta, opcje zmiany hasła, informacje kontaktowe.
- **Kluczowe komponenty widoku:** Lista historii aktywności, formularze do aktualizacji danych, przyciski akcji, mechanizm snackbar.
- **UX, dostępność i względy bezpieczeństwa:** Jasny podział sekcji, łatwy dostęp do najważniejszych funkcji, zabezpieczenia danych osobowych.

### Widok sesji powtórek

- **Nazwa widoku:** Sesja powtórek
- **Ścieżka widoku:** /nauka
- **Główny cel:** Umożliwienie efektywnej nauki fiszek zgodnie z algorytmem spaced repetition.
- **Kluczowe informacje do wyświetlenia:** Fiszka (przód i tył), przyciski do odsłonięcia odpowiedzi oraz oceny stopnia opanowania, informacje o postępach.
- **Kluczowe komponenty widoku:** Mechanizm prezentacji fiszek, przyciski interakcji, wskaźniki postępu, timer (opcjonalnie).
- **UX, dostępność i względy bezpieczeństwa:** Prosty i intuicyjny interfejs, minimalizm dla skupienia użytkownika, możliwość regulacji wielkości czcionki i kontrastu.

## 3. Mapa podróży użytkownika

1. Użytkownik rozpoczyna pracę od ekranu autoryzacji (/auth), gdzie loguje się lub rejestruje.
2. Po pomyślnym logowaniu następuje przekierowanie do Dashboardu (/dashboard), który prezentuje podsumowanie stanu fiszek oraz statystyki.
3. Z dashboardu użytkownik wybiera jeden z głównych modułów:
   - Generowanie fiszek AI (/generowanie) – wprowadza tekst, przegląda wygenerowane propozycje, edytuje/akceptuje fiszki i zapisuje je zbiorczo.
   - Lista fiszek (/fiszki) – przegląda, edytuje lub usuwa istniejące fiszki z asynchroniczną aktualizacją interfejsu.
   - Sesja powtórek (/nauka) – przystępuje do nauki, gdzie wyświetlane są fiszki według algorytmu spaced repetition.
4. Użytkownik może również uzyskać dostęp do panelu użytkownika (/konto) dla zarządzania profilem i przeglądania historii aktywności.
5. Nawigacja między widokami odbywa się głównie poprzez boczne menu lub górny pasek nawigacyjny, umożliwiający szybkie przechodzenie do kluczowych modułów.

## 4. Układ i struktura nawigacji

- Główne nawigowanie odbywa się przez responsywny boczny pasek (drawer) lub nagłówek z menu hamburger.
- Menu zawiera skróty do widoków: Autoryzacja, Dashboard, Generowanie fiszek AI, Lista fiszek, Panel użytkownika oraz Sesja powtórek.
- Na mniejszych urządzeniach, menu zmienia się na rozwijane, aby zapewnić optymalizację przestrzeni ekranu.
- Elementy nawigacyjne są dobrze oznaczone ikonami oraz opisami, co zwiększa dostępność i intuicyjność korzystania.

## 5. Kluczowe komponenty

- **Formularze autoryzacji:** Komponenty umożliwiające logowanie i rejestrację z walidacją inline i zabezpieczeniami.
- **Komponent Dashboardu:** Karty statystyk, wykresy i widgety do szybkiego podglądu stanu konta.
- **Komponent generowania fiszek AI:** Pole tekstowe, lista propozycji fiszek, opcje akcji (akceptacja/edycja/odrzucenie) oraz zbiorczy zapis.
- **Komponent listy fiszek:** Tabela lub dynamiczna lista z modalami do edycji i potwierdzeniami usunięcia.
- **Komponent sesji powtórek:** Interfejs prezentujący fiszki z opcją odsłonięcia odpowiedzi i ocen użytkownika.
- **Snackbar:** Globalny mechanizm komunikacji, informujący o błędach i sukcesach operacyjnych.
- **Nawigacja (Sidebar/Topbar):** Struktura umożliwiająca łatwe przechodzenie między widokami. 