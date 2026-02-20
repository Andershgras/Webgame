# Webgame (ASP.NET Core Web API)

A learning-focused backend project for a simple web-based incremental / click game.
The main goal is to understand and apply **clean architecture**, **persistence patterns**, and
**robust API design** using real-world practices.

---

## Goals

- Build a maintainable backend for a click / upgrade style game
- Learn architecture patterns used in professional .NET projects
- Keep a clean separation between **Domain**, **Application**, and **Infrastructure**
- Support persistence, error handling, and future scalability

---

## Tech Stack

- **.NET 8** – ASP.NET Core Web API  
- **Entity Framework Core** – ORM & migrations  
- **MSSQL** – Remote database  
- **Swagger / OpenAPI** – API testing  
- **Git & GitHub** – Version control  

---

## Solution Structure (Clean Architecture)

The solution follows a Clean Architecture–inspired structure.

### `Webgame.Domain`
**Pure business logic and game rules**

- Entities and value objects (`Player`, `Stats`, `PlayerId`)
- Domain behavior (`Click()`, `TrySpendCoins()`)
- No dependencies on other projects

> Rule: Domain must stay framework-agnostic.

---

### `Webgame.Application`
**Use cases and orchestration**

- Application services (`PlayerService`)
- Interfaces (`IPlayerRepository`, `IUnitOfWork`)
- Result pattern (`Result`, `Error`) for predictable flow control

> Rule: Application depends on Domain, never Infrastructure.

---

### `Webgame.Infrastructure`
**Technical implementation details**

- `WebgameDbContext` (EF Core)
- Repository implementations (`EfPlayerRepository`)
- Unit of Work (`EfUnitOfWork`)

> Rule: Infrastructure depends on Application + Domain.

---

### `Webgame.Api`
**HTTP & presentation layer**

- Controllers and DTOs
- Central Result → HTTP mapping
- Global exception handling
- Request logging

> Rule: Controllers are thin and contain no business logic.

---

## Key Architectural Decisions

### Result Pattern
All application use cases return `Result` or `Result<T>`.

- Avoids exception-driven flow
- Makes errors explicit and testable
- Enables clean HTTP mapping

---

### Repository + Unit of Work
Persistence is explicit and controlled.

- Repositories only track changes
- **Only Unit of Work calls `SaveChangesAsync()`**
- One transaction per use case

---

### Central Error → HTTP Mapping
All application errors are mapped consistently to HTTP responses.

- Uses `ProblemDetails`
- Same format across all endpoints
- Controllers do not manually construct error responses

---

### Global Exception Handling & Logging
Unhandled exceptions are handled centrally.

- Unexpected errors → `500`
- DB constraint violations → `409`
- DB unavailable → `503`
- Each error includes a `traceId`
- All requests are logged with duration and status

---

## Running the Project

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 (recommended)
- MSSQL database (local or remote)

---

### API Endpoints
- POST /api/players – Create player
- GET /api/players/{id} – Get player
- POST /api/players/{id}/click – Perform click action
- DELETE /api/players/{id} – Delete player
- Swagger is available in development at /swagger

### Conventions & Rules:
To keep the architecture consistent:
- Controllers only call Application services
- Application services return Result
- Controllers use central Result → HTTP mapping
- Repositories never call SaveChangesAsync
- Unit of Work controls persistence
- Secrets never go into Git
- Errors are always returned as ProblemDetails

### Branching Strategy:
- main – Stable baseline
- dev – Active development
- feature/* – Feature branches merged into dev

### Notes:
This project is designed as a learning project, but follows
real-world patterns to ensure it can scale as features are added.