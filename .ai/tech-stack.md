Frontend
Blazor Server – wykorzystaj interaktywny model renderowania, w którym logika aplikacji działa po stronie serwera, co umożliwia szybkie i responsywne interakcje z użytkownikiem.

MudBlazor – dodaj tę bibliotekę komponentów inspirowanych Material Design, aby szybko budować nowoczesne i estetyczne interfejsy użytkownika. Instalacja pakietu NuGet w projekcie klienta oraz dodanie przestrzeni nazw w pliku _imports.razor zapewni dostęp do gotowych komponentów, takich jak przyciski, tabele czy karty.

Backend i Baza Danych
ASP.NET Core – wykorzystaj ten framework do budowy API oraz logiki biznesowej, co umożliwi skalowalną i bezpieczną obsługę po stronie serwera.

Entity Framework Core (EF Core) – zastosuj EF Core jako narzędzie ORM, aby uprościć dostęp do danych oraz zarządzanie operacjami na bazie, pamiętając o specyficznych wymaganiach dotyczących kontekstu i cykli życia DbContext w aplikacjach Blazor Server.

PostgreSQL – wykorzystaj PostgreSQL jako system bazodanowy, który zapewnia wysoką niezawodność i skalowalność oraz jest dobrze wspierany przez EF Core. W pliku appsettings.json określ odpowiednią konfigurację połączenia, dostosowaną do środowiska produkcyjnego lub deweloperskiego.

Komunikacja z Modelami AI
Openrouter.ai – zaimplementuj dedykowany serwis, który korzysta z HttpClient w ASP.NET Core, aby komunikować się z API Openrouter.ai. Dzięki temu Twoja aplikacja będzie mogła wysyłać zapytania do modeli AI i odbierać rezultaty, integrując funkcjonalności uczenia maszynowego lub przetwarzania języka naturalnego.

CI/CD i Hosting
GitHub Actions – skonfiguruj pipeline, który automatycznie buduje, testuje i wdraża aplikację. Plik YAML określi kroki budowy obrazu Docker, uruchomienie testów oraz publikację artefaktów, co usprawnia cykl rozwoju oprogramowania.

DigitalOcean – wybierz DigitalOcean do hostingu aplikacji, korzystając z opcji wdrożeń kontenerowych lub maszyn wirtualnych. DigitalOcean umożliwia skalowanie aplikacji oraz szybkie wdrożenia dzięki zautomatyzowanym workflowom, co stanowi doskonałe uzupełnienie dla konfiguracji CI/CD przeprowadzanej przez GitHub Actions.
Podsumowując, ten stack technologiczny łączy interaktywny frontend oparty na Blazor Server oraz MudBlazor z niezawodnym backendem ASP.NET Core wykorzystującym EF Core i PostgreSQL. Dzięki integracji z Openrouter.ai aplikacja może komunikować się z modelami AI, a zautomatyzowany proces CI/CD w GitHub Actions oraz hosting na DigitalOcean zapewniają ciągłość wdrożeń i skalowalność rozwiązania.