# RaceDay

RaceDay is a system that manages road running, walking, and cycling events all across South Africa. Anyone that feels that they want to take part in the event can sign up and be in the next race.

> This repository contains **Part 2 (API)** and **Part 3 (MVC Web App)**. 

## Roles
- **Organiser** - creates, edits, and deletes events, manages event categories, uploads event banners, and captures participant results.
- **Participant** - browses events, enters events by selecting a category, and views their own results.

## Authentication
Users register and log in with email and password. Passwords are hashed with BCrypt. The MVC app maintains the user's authenticated state and role across all pages using session management and API cookies. Unauthenticated users cannot access protected pages.

## Architecture (Part 3)
- **RaceDay (API):** Handles all business logic, database connections (EF Core), session management, and Azure Blob Storage integration for image uploads.
- **RaceDay.Web (MVC):** The frontend user interface. It communicates *exclusively* with the API using `HttpClient` and session cookies. It contains **no direct database access**.
- **Azure Blob Storage:** Integrated into the API. The MVC app sends the image file to the API, and the API uploads it to Azure and stores the URL in the database.

## Setup and run locally

**Requirements**
- .NET 10 SDK
- SQL Server (LocalDB or a full instance)
- EF Core tools: `dotnet tool install --global dotnet-ef`
- Azure Storage Account (for image uploads)

**Steps**
1. Clone the repo:
   `git clone https://github.com/Lupinlj/Prog-2026-Term-2.git`
   `cd Prog-2026-Term-2`
2. Open `RaceDay/appsettings.json` and set:
   - The SQL Server connection string for your machine.
   - The `AzureStorage:ConnectionString` for your Azure Blob container (named `event-images`).
3. Create the database (run from the API project folder):
   `dotnet ef database update`
4. In Visual Studio, right-click the Solution -> **Configure Startup Projects** -> Select **Multiple startup projects** -> Set both `RaceDay` (API) and `RaceDay.Web` (MVC) to **Start**.
5. Press `F5` to run. Swagger will open for the API, and the browser will open for the MVC app.

## 🐳 How to Run with Docker

The MVC application is containerized using Docker. To build and run the application, use the following commands from the root directory:

```bash
# Build the Docker image
docker build -t raceday-web -f RaceDay.Web/Dockerfile .

# Run the container
docker run -d -p 8080:80 --name raceday-app raceday-web
