# Implementation Plan: FinanzApp Estudiantil

**Branch**: `001-student-finance-app` | **Date**: 2026-08-20 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-student-finance-app/spec.md`

## Summary

Build a personal finance web app for university students using ASP.NET Core 8
MVC with Layered Architecture. Single project with Controllers, Razor Views,
Services, Repositories, and EF Core. Real authentication via ASP.NET Core
Identity. All expense, category, and savings goal data persisted in the database.
Charts via Chart.js integrated in Razor Views. Reusable partial views for
common UI patterns. Mobile-first design following the constitution's
green/purple/yellow palette.

## Technical Context

**Language/Version**: C# 12 / .NET 8

**Architecture**: MVC + 4-Layer Separation

```text
Presentation (Controllers + Views + ViewModels)
    ↓
Business Logic (Services)
    ↓
Data Access (Repositories + DbContext)
    ↓
Domain (Entities)
```

**Primary Dependencies**:
- ASP.NET Core 8 MVC (Razor Views, Tag Helpers, model binding)
- Entity Framework Core 8 (SQL Server provider)
- ASP.NET Core Identity (cookie auth)
- Chart.js (client-side charts via CDN)
- Tailwind CSS v4 (standalone CLI)

**Database**: SQL Server (LocalDB for dev, Azure SQL for prod)

**Storage**: SQL Server via EF Core migrations

**Testing**: xUnit (unit + integration) — deferred to dedicated test phase

**Target Platform**: Web (server-rendered MVC), responsive mobile-first

**Performance Goals**: Dashboard loads <3s on 3G, supports 500+ expense records per user

**Constraints**: Mobile-first responsive, WCAG 2.1 AA, constitution visual palette

**Scale/Scope**: ~10 controllers, ~20 views, single-user personal finance (no multi-tenant)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Evidence |
|-----------|--------|----------|
| Enfoque en el Usuario Estudiante | PASS | Simple forms, no jargon, friendly tone throughout spec |
| Consistencia Visual | PASS | Green palette with purple/yellow accents, flat design, rounded typography specified |
| Claridad de Datos | PASS | Charts with labels/tooltips, empty states, 5-second understanding goal |
| Consejos de IA | PASS | FR-011 requires data-specific tips, FR-012 handles insufficient data |
| Privacidad | PASS | FR-020 encryption, FR-019 account deletion, Identity auth |
| Mobile-First | PASS | FR-022 responsive, all stories include mobile scenarios |
| Rendimiento | PASS | SC-003 <3s load, SC-006 500+ records, lazy loading plan |

**Gate Result**: PASS — no violations detected.

## Security & Deployment Prerequisites

**Encryption in transit (FR-020, Constitution P5)**: The application MUST be
deployed behind HTTPS/TLS. All traffic between client and server MUST be
encrypted. This is a deployment/infrastructure configuration, not an
application-level code concern.

- **Production**: Reverse proxy (nginx, Azure App Service, IIS) terminates TLS.
  Certificate management is the operator's responsibility.
- **Development**: ASP.NET Core development certificate (`dotnet dev-certs
  https`) provides localhost HTTPS. HTTP-to-HTTPS redirect is configured in
  `Program.cs` via `app.UseHttpsRedirection()`.

**Encryption at rest (FR-020, Constitution P5)**: SQL Server data-at-rest
encryption (TDE or Azure SQL Always Encrypted) is configured at the database
server level, not in application code.

No application-level encryption code is required — encryption is handled by
the hosting infrastructure and database server.

## Project Structure

### Documentation (this feature)

```text
specs/001-student-finance-app/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   └── api-contracts.md
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root)

```text
src/
└── FinanzApp.Web/                    # ASP.NET Core MVC (single project)
    ├── Controllers/                  # Presentation: HTTP handlers
    │   ├── HomeController.cs         # Landing page
    │   ├── AuthController.cs         # Login, Register, Logout
    │   ├── DashboardController.cs    # Dashboard + Budget
    │   ├── ExpenseController.cs      # Expense CRUD
    │   ├── GoalController.cs         # Savings goals
    │   └── TipController.cs          # AI tips
    ├── Views/                        # Presentation: Razor Views
    │   ├── Shared/
    │   │   ├── _Layout.cshtml
    │   │   ├── _ValidationScriptsPartial.cshtml
    │   │   └── _ViewImports.cshtml
    │   ├── Home/
    │   │   └── Index.cshtml
    │   ├── Auth/
    │   │   ├── Login.cshtml
    │   │   └── Register.cshtml
    │   ├── Dashboard/
    │   │   └── Index.cshtml
    │   ├── Expense/
    │   │   ├── Index.cshtml
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
    │   ├── Category.cs              # Enum
    │   ├── SavingsGoal.cs
    │   ├── SavingsEntry.cs
    │   └── AITip.cs
    ├── ViewModels/                   # Presentation: ViewModels
    │   ├── DashboardViewModel.cs
    │   ├── ExpenseFormViewModel.cs
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
    │   ├── IBudgetService.cs
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
    │   ├── js/                       # Chart.js scripts
    │   └── images/
    ├── Program.cs                    # DI, middleware, MVC config
    └── FinanzApp.Web.csproj

tests/
└── FinanzApp.Web.Tests/             # xUnit tests
    ├── Controllers/
    ├── Services/
    └── Repositories/

db/                                  # SQL scripts (schema, triggers, SPs)
```

**Structure Decision**: Single ASP.NET Core MVC project with layered
architecture. All presentation, business logic, data access, and domain
code lives in one deployable unit. No separate frontend/backend — the
server renders Razor Views and handles form submissions directly.

## Layer Responsibilities

| Layer | Folders | Rules |
|-------|---------|-------|
| Presentation | `Controllers/`, `Views/`, `ViewModels/` | Handle HTTP, never access DbContext, delegate to Services |
| Business Logic | `Services/` | All business rules, injected via DI, interface + implementation |
| Data Access | `Repositories/`, `Data/` | EF Core queries, never contain business logic |
| Domain | `Models/` | Pure entities, enums, Data Annotations, no dependencies |

## Dependency Injection Order (Program.cs)

```csharp
// 1. DbContext
builder.Services.AddDbContext<AppDbContext>(...);

// 2. Repositories
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<ITipRepository, TipRepository>();

// 3. Services
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<ITipService, TipService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

// 4. Identity + MVC
builder.Services.AddIdentity<...>(...);
builder.Services.AddControllersWithViews();
```

## Complexity Tracking

No constitution violations to justify. All technical choices align with the
constitution principles and user-specified stack.
