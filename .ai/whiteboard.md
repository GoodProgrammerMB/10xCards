```mermaid
flowchart LR
subgraph Proces_Nauki["Nauka z fiszkami v1"]
    %% Actors
    ACT1((Użytkownik))
    ACT2((System))
    
    %% Commands
    CMD1[Wygeneruj fiszki]
    CMD2[Zatwierdź fiszki]
    CMD3[Wylosuj zestaw do nauki]
    CMD4[Udziel odpowiedzi]
    CMD5[Sprawdź fiszki do powtórki]
    
    %% Read Models
    RM1[(Lista dostępnych fiszek)]
    RM2[(Historia powtórek)]
    RM3[(Treść aktualnej fiszki)]
    RM4[(Statystyki nauki)]
    
    %% Aggregates
    AGG1{Fiszka}
    AGG2{Sesja nauki}
    AGG3{Harmonogram powtórek}
    
    %% Events
    DE1[Fiszki zostały wygenerowane]
    DE2[Zestaw fiszek został wylosowany]
    DE3[Fiszka została oznaczona jako przerobiona]
    DE4[Fiszki do powtórki zostały dodane]
    DE5[Fiszka została oznaczona do powtórki]
    DE6[Data powtórki została zaktualizowana]
    DE7[Sesja nauki została zakończona]
    DE8[Fiszki zostały oznaczone jako gotowe do powtórki]
    DE9[Odpowiedź na fiszkę została udzielona]
    DE10[Licznik powtórek został zaktualizowany]
    
    %% Connections
    ACT1 -->|inicjuje| CMD1
    CMD1 -.-> DE1
    ACT1 -->|wykonuje| CMD2
    CMD2 -.-> DE1
    
    ACT2 -->|wykonuje| CMD3
    CMD3 -.-> DE2
    
    ACT1 -->|wykonuje| CMD4
    CMD4 -.-> DE9
    
    ACT2 -->|wykonuje| CMD5
    CMD5 -.-> DE8
    
    %% Read Model connections
    CMD3 -.->|czyta| RM1
    CMD3 -.->|czyta| RM2
    CMD4 -.->|czyta| RM3
    ACT1 -.->|przegląda| RM4
    
    %% Aggregate connections
    CMD1 -.->|tworzy| AGG1
    CMD3 -.->|tworzy| AGG2
    AGG2 -->|zarządza| AGG1
    AGG3 -->|planuje powtórki| AGG1
    
    DE1 --> DE2
    DE2 --> DE9
    DE9 --> DE3
    DE3 --> DE5
    DE5 --> DE4
    DE3 --> DE6
    DE3 --> DE10
    DE3 --> DE7
    DE8 --> DE4
    
    %% Styles
    style ACT1 fill:#FFFF00,color:black
    style ACT2 fill:#FFFF00,color:black
    
    style CMD1 fill:#1E90FF,color:white
    style CMD2 fill:#1E90FF,color:white
    style CMD3 fill:#1E90FF,color:white
    style CMD4 fill:#1E90FF,color:white
    style CMD5 fill:#1E90FF,color:white
    
    style RM1 fill:#32CD32,color:black
    style RM2 fill:#32CD32,color:black
    style RM3 fill:#32CD32,color:black
    style RM4 fill:#32CD32,color:black
    
    style AGG1 fill:#FFFF00,color:black
    style AGG2 fill:#FFFF00,color:black
    style AGG3 fill:#FFFF00,color:black
    
    style DE1 fill:#FF9900,color:black
    style DE2 fill:#FF9900,color:black
    style DE3 fill:#FF9900,color:black
    style DE4 fill:#FF9900,color:black
    style DE5 fill:#FF9900,color:black
    style DE6 fill:#FF9900,color:black
    style DE7 fill:#FF9900,color:black
    style DE8 fill:#FF9900,color:black
    style DE9 fill:#FF9900,color:black
    style DE10 fill:#FF9900,color:black
end

subgraph Legenda
  DE0[Domain Event]
  CMD0[Command]
  RM0[(Read Model)]
  POL0>Policy]
  AGG0{Aggregate}
  HS0/!/
  ACT0((Actor))
  EX0{{External System}}

  style DE0 fill:#FF9900,color:black
  style CMD0 fill:#1E90FF,color:white
  style RM0 fill:#32CD32,color:black
  style POL0 fill:#9932CC,color:white
  style AGG0 fill:#FFFF00,color:black
  style HS0 fill:#FF0000,color:white
  style ACT0 fill:#FFFF00,color:black
  style EX0 fill:#A9A9A9,color:white
end