# Implementation Plan: FinanzApp Estudiantil

**Branch**: `001-student-finance-app` | **Date**: 2026-08-20 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-student-finance-app/spec.md`

## Summary

Build a full-stack personal finance web app for university students. Backend
in ASP.NET Core Web API with EF Core + SQL Server. Frontend in Blazor
WebAssembly consuming the API. Real authentication via ASP.NET Core Identity.
All expense, category, and savings goal data persisted in the database.
Charts via ApexCharts.Blazor for donut and line visualizations. Reusable
component architecture (SummaryCard, ExpenseForm, AITipCard). Mobile-first
design following the constitution's green/purple/yellow palette.

## Technical Context

**Language/Version**: C# 12 / .NET 8

**Primary Dependencies**:
- Backend: ASP.NET Core 8 Web API, Entity Framework Core 8, ASP.NET Core Identity
- Frontend: Blazor WebAssembly (.NET 8), ApexCharts.Blazor
- Database: SQL Server (LocalDB for dev, Azure SQL for prod)

**Storage**: SQL Server via EF Core migrations

**Testing**: xUnit, bUnit (Blazor component tests), Integration tests via WebApplicationFactory — deferred to dedicated test phase after all user stories are implemented (see tasks T090–T095)

**Target Platform**: Web (Blazor WASM in browser), responsive mobile-first

**Project Type**: Web application (frontend + backend)

**Performance Goals**: Dashboard loads <3s on 3G, supports 500+ expense records per user

**Constraints**: Mobile-first responsive, WCAG 2.1 AA, constitution visual palette

**Scale/Scope**: ~50 screens/components, single-user personal finance (no multi-tenant)

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
backend/
├── src/
│   ├── FinanzApp.Api/           # ASP.NET Core Web API project
│   │   ├── Controllers/         # API controllers
│   │   ├── Models/              # EF Core entity models
│   │   ├── Data/                # DbContext, migrations
│   │   ├── Services/            # Business logic, AI tip generation
│   │   └── Program.cs
│   └── FinanzApp.Api.Tests/     # xUnit integration tests
└── tests/
    └── integration/

frontend/
├── src/
│   ├── FinanzApp.Web/           # Blazor WebAssembly project
│   │   ├── Components/         # Reusable UI components
│   │   │   ├── SummaryCard.razor
│   │   │   ├── ExpenseForm.razor
│   │   │   ├── AITipCard.razor
│   │   │   ├── GoalCard.razor
│   │   │   └── Charts/
│   │   │       ├── DonutChart.razor
│   │   │       └── LineChart.razor
│   │   ├── Pages/              # Route pages
│   │   │   ├── Landing.razor
│   │   │   ├── Login.razor
│   │   │   ├── Register.razor
│   │   │   ├── Dashboard.razor
│   │   │   ├── Expenses.razor
│   │   │   ├── Tips.razor
│   │   │   └── Goals.razor
│   │   ├── Services/           # API client services
│   │   ├── Models/             # DTOs / view models
│   │   └── wwwroot/
│   │       ├── css/            # Global styles (green palette)
│   │       └── images/         # Flat design illustrations
│   └── FinanzApp.Web.Tests/    # bUnit component tests
└── tests/
    └── unit/
```

**Structure Decision**: Option 2 (Web application) — separate backend API
and frontend Blazor WASM projects. Backend handles auth, data, and AI tip
generation. Frontend consumes the API and provides the mobile-first UI.

## Complexity Tracking

No constitution violations to justify. All technical choices align with the
constitution principles and user-specified stack.
