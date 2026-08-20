# AGENTS.md — FinanzApp Estudiantil

Personal finance web app for university students. Students track expenses (mensualidad, transporte, comida, entretenimiento) and receive AI-generated savings tips.

## Tech Stack

- **Backend**: C# 12 / ASP.NET Core 8 Web API
- **Frontend**: Blazor WebAssembly (.NET 8)
- **Database**: SQL Server (LocalDB dev, Azure SQL prod)
- **ORM**: Entity Framework Core 8
- **Auth**: ASP.NET Core Identity
- **Charts**: ApexCharts.Blazor
- **CSS**: Tailwind CSS v4
- **Tests**: xUnit, bUnit

## Project Structure

```text
backend/
├── src/
│   ├── FinanzApp.Api/           # ASP.NET Core Web API
│   │   ├── Controllers/
│   │   ├── Models/              # EF Core entity models
│   │   ├── Data/                # DbContext, migrations
│   │   ├── Services/            # Business logic, AI tip generation
│   │   ├── Middleware/
│   │   └── Program.cs
│   └── FinanzApp.Api.Tests/     # xUnit integration tests
└── tests/
    └── integration/

frontend/
├── src/
│   ├── FinanzApp.Web/           # Blazor WebAssembly
│   │   ├── Components/
│   │   │   ├── Shared/          # Reusable UI components
│   │   │   ├── Layout/
│   │   │   └── Pages/           # Route pages
│   │   ├── Services/            # API client services
│   │   ├── Models/              # DTOs / view models
│   │   └── wwwroot/
│   └── FinanzApp.Web.Tests/     # bUnit component tests
└── tests/
    └── unit/

db/                              # SQL scripts (schema, triggers, SPs)
specs/001-student-finance-app/   # Feature specs, plans, tasks
```

## Build & Run

```bash
# Backend
cd backend/src/FinanzApp.Api
dotnet restore
dotnet build

# Frontend
cd frontend/src/FinanzApp.Web
dotnet restore
dotnet build
```

## Test Commands

```bash
# Backend integration tests
cd backend/tests/FinanzApp.Api.Tests
dotnet test

# Frontend bUnit tests
cd frontend/tests/FinanzApp.Web.Tests
dotnet test
```

## Code Style

- C# 12, .NET 8
- Use Data Annotations for validation (`[Required]`, `[StringLength]`, `[Range]`, `[ForeignKey]`)
- Entity models in `backend/src/FinanzApp.Api/Models/`
- DTOs in `frontend/src/FinanzApp.Web/Models/`
- Services handle business logic, not controllers
- All UI text in friendly, non-corporate Spanish
- Mobile-first responsive design
- WCAG 2.1 AA accessibility (keyboard nav, aria labels, 4.5:1 contrast)

## Key Conventions

- **Currency**: Locked at registration, displayed consistently everywhere (FR-033)
- **Cascade delete**: User deletion removes all expenses, goals, tips (FR-019)
- **AI Tips**: Must reference specific user data, never generic (FR-011, FR-012)
- **Charts**: Must have descriptive alt text for screen readers (FR-035)
- **Empty states**: All sections show helpful guidance when no data exists (FR-021)
- **Dashboard**: Loads <3s, supports 500+ expense records (SC-003, SC-006)

## Database

SQL scripts in `db/` directory:
1. `01-crear-schema.sql` — Tables and schema
2. `03-procedimientos-almacenados.sql` — Stored procedures
3. `04-triggers.sql` — Audit triggers
4. `05-insercion-masiva.sql` — Seed data
5. `06-vistas.sql` — Views
6. `07-testeo.sql` — Test queries

## Implementation Phases

1. Setup (project init)
2. Foundational (Identity, DbContext, auth — BLOCKS all features)
3. US1: Landing & Registration
4. US2: Dashboard Overview
5. US3: Expense Registration
6. US4: AI Financial Tips
7. US5: Savings Goals & Projection
8. Polish & Cross-Cutting
9. Testing (deferred)

## PR Instructions

- Title format: `[Phase] Description`
- Run `dotnet build` and `dotnet test` before committing
- Commit after each task or logical group
- Stop at checkpoints to validate stories independently
