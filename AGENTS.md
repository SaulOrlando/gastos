# AGENTS.md — FinanzApp Estudiantil

Personal finance web app for university students. Students track expenses (mensualidad, transporte, comida, entretenimiento) and receive AI-generated savings tips.

## Architecture

**Pattern**: MVC (Model-View-Controller) + Layered Architecture (3 capas)

```text
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│   Controllers  ──→  Razor Views (.cshtml)│
│         Models (ViewModels / DTOs)       │
├─────────────────────────────────────────┤
│         Business Logic Layer            │
│         Services (interfaces + impl)    │
├─────────────────────────────────────────┤
│         Data Access Layer               │
│   Repositories  ──→  DbContext (EF Core) │
├─────────────────────────────────────────┤
│         Domain Layer                    │
│         Entities (EF Core models)       │
└─────────────────────────────────────────┘
```

**Rules**:
- Controllers ONLY handle HTTP and delegate to Services
- Services contain ALL business logic, never access DbContext directly
- Repositories wrap EF Core queries, never contain business logic
- Razor Views use `@model` with ViewModels/DTOs, never entities directly
- No cross-layer violations: Presentation → Business → Data → Domain

## Tech Stack

- **Framework**: C# 12 / ASP.NET Core 8 MVC
- **Database**: SQL Server (LocalDB dev, Azure SQL prod)
- **ORM**: Entity Framework Core 8
- **Auth**: ASP.NET Core Identity
- **CSS**: Tailwind CSS v4
- **Charts**: Librería JS (Chart.js o similar, integrada en Razor Views)
- **Tests**: xUnit

## Project Structure

```text
src/
└── FinanzApp.Web/                    # ASP.NET Core MVC (single project)
    ├── Controllers/                  # Presentation: handle HTTP requests
    │   ├── HomeController.cs
    │   ├── AuthController.cs
    │   ├── DashboardController.cs
    │   ├── ExpenseController.cs
    │   ├── GoalController.cs
    │   └── TipController.cs
    ├── Views/                        # Presentation: Razor views (.cshtml)
    │   ├── Shared/
    │   │   ├── _Layout.cshtml        # Master layout
    │   │   ├── _ValidationScriptsPartial.cshtml
    │   │   └── _ViewImports.cshtml
    │   ├── Home/
    │   │   └── Index.cshtml          # Landing page
    │   ├── Auth/
    │   │   ├── Login.cshtml
    │   │   └── Register.cshtml
    │   ├── Dashboard/
    │   │   └── Index.cshtml
    │   ├── Expense/
    │   │   ├── Index.cshtml          # List expenses
    │   │   ├── Create.cshtml
    │   │   ├── Edit.cshtml
    │   │   └── Delete.cshtml
    │   ├── Goal/
    │   │   ├── Index.cshtml
    │   │   ├── Create.cshtml
    │   │   └── AddSavings.cshtml
    │   └── Tip/
    │       └── Index.cshtml
    ├── Models/                       # Domain: EF Core entities
    │   ├── User.cs
    │   ├── Expense.cs
    │   ├── SavingsGoal.cs
    │   ├── SavingsEntry.cs
    │   └── AITip.cs
    ├── ViewModels/                   # Presentation: ViewModels / DTOs
    │   ├── DashboardViewModel.cs
    │   ├── ExpenseViewModel.cs
    │   ├── GoalViewModel.cs
    │   └── TipViewModel.cs
    ├── Services/                     # Business Logic Layer
    │   ├── IDashboardService.cs
    │   ├── DashboardService.cs
    │   ├── IExpenseService.cs
    │   ├── ExpenseService.cs
    │   ├── IGoalService.cs
    │   ├── GoalService.cs
    │   ├── ITipService.cs
    │   ├── TipService.cs
    │   └── BudgetService.cs
    ├── Repositories/                 # Data Access Layer
    │   ├── IExpenseRepository.cs
    │   ├── ExpenseRepository.cs
    │   ├── IGoalRepository.cs
    │   ├── GoalRepository.cs
    │   ├── ITipRepository.cs
    │   └── TipRepository.cs
    ├── Data/                         # Data Access: DbContext, Migrations
    │   ├── AppDbContext.cs
    │   └── Migrations/
    ├── wwwroot/
    │   ├── css/                      # Tailwind CSS output
    │   ├── js/                       # Chart.js or similar
    │   └── images/
    ├── Program.cs
    └── FinanzApp.Web.csproj
```

## Build & Run

```bash
cd src/FinanzApp.Web
dotnet restore
dotnet build
dotnet run
```

## Test Commands

```bash
cd tests/FinanzApp.Web.Tests
dotnet test
```

## Code Style

- C# 12, .NET 8
- MVC pattern: Controllers → Services → Repositories → DbContext
- Use Data Annotations for validation (`[Required]`, `[StringLength]`, `[Range]`)
- Entities in `Models/` (EF Core, never in Views)
- ViewModels in `ViewModels/` (one per view or shared across related views)
- Services: interface + implementation, injected via DI
- Repositories: interface + implementation, wrap EF Core
- Razor Views use `@model` with ViewModels, never with entities
- All UI text in friendly, non-corporate Spanish
- Mobile-first responsive design
- WCAG 2.1 AA accessibility (keyboard nav, aria labels, 4.5:1 contrast)

## Layer Responsibilities

| Layer | Folder | Responsibility | Dependencies |
|-------|--------|----------------|--------------|
| Presentation | `Controllers/`, `Views/`, `ViewModels/` | Handle HTTP, render UI | Business Logic |
| Business Logic | `Services/` | Business rules, orchestration | Data Access, Domain |
| Data Access | `Repositories/`, `Data/` | EF Core queries, migrations | Domain |
| Domain | `Models/` | Entities, enums | None |

## Key Conventions

- **Currency**: Locked at registration, displayed consistently everywhere (FR-033)
- **Cascade delete**: User deletion removes all expenses, goals, tips (FR-019)
- **AI Tips**: Must reference specific user data, never generic (FR-011, FR-012)
- **Charts**: Must have descriptive alt text for screen readers (FR-035)
- **Empty states**: All sections show helpful guidance when no data exists (FR-021)
- **Dashboard**: Loads <3s, supports 500+ expense records (SC-003, SC-006)

## Database

SQL Server via EF Core migrations. Create/update with:
```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Implementation Phases

1. Setup (project init, Tailwind, Identity)
2. Foundational (DbContext, auth, DI — BLOCKS all features)
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
