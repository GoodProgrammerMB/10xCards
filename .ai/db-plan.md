## Schemat Bazy Danych PostgreSQL dla MVP

### 1. Tabele

#### 1.1 Users
- `id` SERIAL PRIMARY KEY
- `email` VARCHAR(255) NOT NULL UNIQUE
- `created_at` TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL
- `updated_at` TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL

#### 1.2 Flashcards
- `id` SERIAL PRIMARY KEY
- `user_id` INTEGER NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- `generation_id` INTEGER REFERENCES Generations(id) ON DELETE SET NULL
- `front` VARCHAR(200) NOT NULL
- `back` VARCHAR(500) NOT NULL
- `source` VARCHAR(20) NOT NULL CHECK (source IN ('ai-full', 'ai-edited', 'manual'))
- `created_at` TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL
- `updated_at` TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL

#### 1.3 Generations
- `id` SERIAL PRIMARY KEY
- `user_id` INTEGER NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- `model` VARCHAR(255) NOT NULL
- `generated_count` INTEGER NOT NULL
- `accepted_unedited_count` INTEGER NULL
- `accepted_edited_count` INTEGER  NULL
- `source_text_hash` VARCHAR(255) NOT NULL
- `source_text_length` INTEGER NOT NULL CHECK (source_text_length BETWEEN 1000 AND 10000)
- `generation_duration` INTEGER NOT NULL
- `created_at` TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL
- `updated_at` TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL

#### 1.4 Generation_Error_Logs
- `id` SERIAL PRIMARY KEY
- `user_id` INTEGER NOT NULL REFERENCES Users(id) ON DELETE CASCADE
- `model` VARCHAR(255) NOT NULL
- `source_text_hash` VARCHAR(255) NOT NULL
- `source_text_length` INTEGER NOT NULL CHECK (source_text_length BETWEEN 1000 AND 10000)
- `error_code` VARCHAR(50) NOT NULL
- `error_message` VARCHAR(500) NOT NULL
- `created_at` TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL

### 2. Relacje między tabelami
- Jeden użytkownik (Users) może posiadać wiele fiszek (Flashcards), generacji (Generations) oraz logów błędów (Generation_Error_Logs).
- `Flashcards.user_id` → `Users.id`
- `Flashcards.generation_id` → `Generations.id` (dla fiszek generowanych przez AI)
- `Generations.user_id` → `Users.id`
- `Generation_Error_Logs.user_id` → `Users.id`

### 3. Indeksy
- Indeks na kolumnie `user_id` w tabelach: Flashcards, Generations, Generation_Error_Logs.
- Indeks na kolumnie `source` w tabeli Flashcards.

### 4. Zasady MS SQL (RLS)
- Na tym etapie zasady zabezpieczeń na poziomie wiersza (RLS) są pominięte.

### 5. Dodatkowe Uwagi
- Walidacja długości pól tekstowych jest egzekwowana poprzez użycie typu VARCHAR(n) oraz dodatkowych CHECK constraints.
- Trigger rejestrujący zmiany w tabeli Flashcards zostanie zaimplementowany poza schematem migracji, aby monitorować operacje na rekordach. 