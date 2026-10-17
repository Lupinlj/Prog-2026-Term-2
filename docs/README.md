# RaceDay

RaceDay is a system that manages road running, walking, and cycling events all across South Africa. Anyone that feels that they want to take part in the event can sign up and be in the next race.

> This repository (Prog-2026-Term-2) contains Part 2 only. All code lives in the repo root.

## Roles
- **Organiser** - creates, edits, and deletes events, manages event categories, and captures participant results.
- **Participant** - browses events, enters events by selecting a category, and views their own results.

## Authentication
Users register and log in with email and password. Passwords are hashed with BCrypt and logins are tracked with a server-side session. The role decides which endpoints a user can use.

## Setup and run

**Requirements**
- .NET 10 SDK
- SQL Server (LocalDB or a full instance)
- EF Core tools: `dotnet tool install --global dotnet-ef`

**Steps**
1. Clone the repo:
```
   git clone https://github.com/Lupinlj/Prog-2026-Term-2.git
   cd Prog-2026-Term-2
```
2. Open `appsettings.json` and set the SQL Server connection string for your machine.
3. Create the database:
```
   dotnet ef database update
```
4. Run the API:
```
   dotnet run
```
5. Open Swagger in your browser (the port is shown in the console): `https://localhost:<port>/swagger`

## Endpoints
All endpoints are documented in Swagger. They cover auth (register, login, logout), events, categories, enrolments, profile and results.

## Unit tests
```
dotnet test RaceDay.slnx
```

## CI/CD
A GitHub Actions workflow (`.github/workflows/dotnet.yml`) builds the project and runs all unit tests on every push.
<img width="1453" height="262" alt="image" src="https://github.com/user-attachments/assets/b7bfcd0a-2422-4149-a8a4-27e28377c9cf" />

