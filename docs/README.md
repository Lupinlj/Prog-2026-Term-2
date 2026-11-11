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
