<mermaid_diagram>
flowchart LR
  U["Użytkownik"]
  L["Formularz Logowania"]
  R["Formularz Rejestracji"]
  O["Formularz Odzyskiwania Hasła"]
  N["Komponent Nawigacji"]
  D["Dashboard"]
  F["Zarządzanie Fiszkami"]
  API["API Autentykacji"]
  ID["ASP.NET Core Identity"]
  AI["Moduł Generowania Fiszek AI"]

  U -- "Loguje się" --> L
  U -- "Rejestruje się" --> R
  U -- "Reset hasła" --> O
  U --> N

  L -- "US-002: Logowanie" --> API
  R -- "US-001: Rejestracja" --> API
  O -- "Inicjacja resetu" --> API

  API ==> ID

  N -.-> D
  D ==> F
  F -- "Generacja" --> AI
</mermaid_diagram> 