# 10x-cards

## Table of Contents
- [Project Description](#project-description)
- [Tech Stack](#tech-stack)
- [Getting Started Locally](#getting-started-locally)
- [Available Scripts](#available-scripts)
- [Project Scope](#project-scope)
- [Project Status](#project-status)
- [License](#license)

## Project Description
10x-cards is a web-based application that streamlines the creation and management of educational flashcards. By leveraging cutting-edge AI via API, the system automatically generates flashcard suggestions from user-provided text, significantly reducing the time and effort required. In addition to AI-driven features, it allows users to manually create, edit, and delete flashcards. The application is designed with secure user authentication and personalized study sessions using spaced repetition, making learning both efficient and effective.

## Tech Stack
- **Frontend:**  
  - Blazor Server for interactive web pages.  
  - MudBlazor for Material Design-inspired UI components.

- **Backend:**  
  - ASP.NET Core with minimal APIs for scalable business logic.  
  - ASP.NET Core Identity for user management and authentication.

- **Database:**  
  - Entity Framework Core paired with MS SQL Server ensures robust data storage and scalability.

- **AI Integration:**  
  - Communicates with Openrouter.ai to generate intelligent flashcard suggestions using HttpClient.

- **Testing:**
  - **Unit Tests:** xUnit with Moq for isolated component testing.
  - **End-to-End Tests:** Playwright for frontend automation and comprehensive user flow validation.
  - **API Tests:** HttpClient for integration testing of backend endpoints.

- **CI/CD & Hosting:**  
  - GitHub Actions for continuous integration and deployment pipelines.
  - DigitalOcean for reliable and scalable hosting.

- **Additional Tools:**  
  - Node.js support (as defined in package.json and .nvmrc) for managing additional dependencies and build tools.

## Getting Started Locally
### Prerequisites
- [.NET 6 or later](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (use the version specified in the `.nvmrc` file)
- MS SQL Server or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- An API key for Openrouter.ai (if you intend to use AI flashcard generation)

### Installation
1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/10x-cards.git
   cd 10x-cards
   ```
2. **Restore and install dependencies:**
   ```bash
   dotnet restore
   npm install
   ```
3. **Configure the project:**
   - Update the connection string in `appsettings.json` to point to your MS SQL Server instance.
   - Set the Openrouter.ai API key in your configuration files or environment variables.

### Running the Application
Start the application with:
```bash
dotnet run
```
The application will be available at [http://localhost:5000](http://localhost:5000) (or on the configured port).

## Available Scripts
### .NET CLI Commands
- `dotnet restore` – Restore all project dependencies.  
- `dotnet build` – Build the application.  
- `dotnet run` – Run the application.  
- `dotnet test` – Run tests (if available).

### Testing Commands
- `./install-playwright.ps1` - Install Playwright browsers for E2E testing.
- `./run-tests.ps1 -TestCategory Unit` - Run only unit tests.
- `./run-tests.ps1 -TestCategory E2E` - Run only E2E tests.
- `./run-tests.ps1` - Run all tests (unit and E2E).

### Node.js Commands (if applicable)
- `npm install` – Install Node.js dependencies.
- `npm run build` – Build frontend assets.
- `npm start` – Start any defined Node.js tools or development servers.

## Project Scope
**Included in the MVP:**
- **Automatic Flashcard Generation:** Paste text and generate flashcard suggestions using AI.
- **Manual Management:** Create, edit, and delete flashcards manually.
- **User Authentication:** Register, log in, and manage user accounts securely.
- **Spaced Repetition Integration:** Schedule flashcard review for effective learning.
- **Analytics:** Track statistics on flashcard generation and usage.

**Out of Scope (for MVP):**
- Advanced spaced repetition algorithms.
- Gamification features.
- Mobile application support.
- Importing multiple document formats (e.g., PDF, DOCX).
- Public API access.
- Extensive notification systems.

## Project Status
The project is currently in the MVP phase and under active development. Future updates will expand functionality and improve the user experience.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for further details. 