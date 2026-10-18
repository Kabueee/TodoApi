# TodoList Web API

Aplikacja RESTful API do zarządzania listą zadań (To-Do).

## 🛠 Technologie i biblioteki
* **Język:** C# (.NET 8 / .NET 9)
* **Framework:** ASP.NET Core (Minimal APIs)
* **ORM:** Entity Framework Core
* **Baza danych:** SQLite
* **Dokumentacja:** Swagger UI / OpenAPI

## 🚀 Co potrafi ta aplikacja?
* **Zarządzanie zadaniami (CRUD):** Możesz dodawać nowe zadania, przeglądać je, edytować tekst, oznaczać jako "wykonane" oraz usuwać (zarówno pojedynczo, jak i wszystkie naraz).
* **Wyszukiwanie i filtry:** Można szukać zadań po słowach kluczowych lub filtrować (pokazać tylko zrobione / niezrobione).
* **Stronicowanie (Paginacja):** Jeśli zadań jest dużo, aplikacja nie pokazuje wszystkich naraz, ale dzieli je na strony (np. po 10 sztuk), żeby serwer działał szybciej.
* **Logowanie (Logging):** Program zapisuje w konsoli informacje o tym, co się dzieje (np. "Dodano nowe zadanie").

## 📋 Jak uruchomić projekt u siebie

### Wymagania
Musisz mieć zainstalowany [.NET SDK](https://microsoft.com) (wersja 8.0 lub nowsza).

### Instrukcja krokiem po kroku
1. **Pobierz kod (sklonuj repozytorium):**
   ```bash
   git clone https://github.com
   cd NAZWA_FOLDERU_PROJEKTU
   ```

2. **Zbuduj projekt i pobierz biblioteki:**
   ```bash
   dotnet restore
   ```

3. **Uruchom program:**
   ```bash
   dotnet run
   ```

4. **Przetestuj w przeglądarce (Swagger):**
   Gdy program się uruchomi, otwórz przeglądarkę i wejdź na:
   ```text
   http://localhost:5087/swagger
   ```
   
# TodoList Web API

A RESTful API for managing a task list (To-Do).

## 🛠 Technologies and Libraries
* **Language:** C# (.NET 8 / .NET 9)
* **Framework:** ASP.NET Core (Minimal APIs)
* **ORM:** Entity Framework Core
* **Database:** SQLite
* **Documentation:** Swagger UI / OpenAPI

## 🚀 Features
* Full CRUD operations for tasks (Create, Read, Update, Delete).
* Text-based task search and filtering by `isDone` status.
* Pagination and dynamic sorting of results.
* Event logging using `ILogger`.

## 📋 How to Run Locally

### Prerequisites
* .NET SDK (version 8.0 or newer).

### Installation and Setup
1. Clone the repository and navigate to the project directory:
   ```bash
   git clone https://github.com
   cd YOUR_PROJECT_FOLDER_NAME
   ```
2. Restore packages and dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Open Swagger UI in your browser to test the API:
   ```text
   http://localhost:5087/swagger
   ```
