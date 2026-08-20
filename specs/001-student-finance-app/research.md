# Research: FinanzApp Estudiantil

**Date**: 2026-08-20

## R1: Charting Library — ApexCharts.Blazor

**Decision**: Use `Blazor-ApexCharts` (NuGet) v7.0.0

**Rationale**:
- Actively maintained (70 releases, 3.2M+ downloads, MIT license)
- Supports Donut, Pie, Line, Area and all other chart types needed
- Explicit .NET 8 WASM support (`net8.0` target)
- Rich interactive features (tooltips, legends, animations)

**Alternatives considered**:
- Chart.js via JS interop: More manual work, requires interop wrapper,
  less Blazor-native integration
- MudBlazor charts: Limited chart types, no native donut support

**Constraints**:
- Must use `rendermode="Interactive"` (InteractiveWebAssembly or
  InteractiveAuto) — does not work in static SSR mode
- Each chart needs its own `ApexChartOptions` instance (not shared)
- Donut chart dynamic updates require `UpdateOptionsAsync(true, true, false)`
  + `RenderAsync()` workaround

## R2: Authentication — ASP.NET Core Identity + Cookies

**Decision**: Cookie-based authentication with `MapIdentityApi()`

**Rationale**:
- Microsoft recommends cookies for browser-based apps (no JS exposure)
- `MapIdentityApi()` in .NET 8 provides minimal API endpoints
  (`/login`, `/register`, `/logout`) without Razor Pages scaffolding
- `CookieHandler` (DelegatingHandler) auto-sends cookies with API requests
- `CookieAuthenticationStateProvider` manages client-side auth state

**Pattern**:
```
Backend:
  AddIdentityCore<User>().AddEntityFrameworkStores<DbContext>()
  .AddApiEndpoints()
  MapIdentityApi<User>()

Frontend:
  CookieHandler -> HttpClient
  CookieAuthenticationStateProvider -> AuthenticationStateProvider
  Login: POST /login?useCookies=true
```

**Known issue**: Role claims not returned from `/manage/info` in .NET 8.
Fetch from separate `/roles` endpoint if roles are needed (not required
for v1 — single student role).

**Alternatives considered**:
- JWT tokens: Overkill for browser apps, adds complexity, tokens exposed
  to JS
- External OAuth (Google, GitHub): Good for v2, adds third-party dependency

## R3: CSS Framework — Tailwind CSS v4

**Decision**: Tailwind CSS v4 with standalone CLI (no Node.js required)

**Rationale**:
- Utility-first approach ideal for custom green/purple/yellow palette
- Config-based theming: define `--color-primary`, `--color-secondary`,
  `--color-accent` tokens directly
- Mobile-first responsive classes built-in (`sm:`, `md:`, `lg:`)
- Generated CSS is 5-7x smaller than Bootstrap (performance aligned with
  constitution Principle 7)
- No Node.js dependency — standalone CLI binary

**Setup**:
1. Download `tailwindcss.exe` from GitHub releases
2. Add MSBuild `BeforeBuild` target in `.csproj`
3. Reference compiled CSS from `index.html`

**Alternatives considered**:
- Bootstrap 5: Ships with Blazor templates, but heavy customization
  needed for constitution palette, larger output
- Blazorise: Component library with Tailwind provider, adds dependency
  and learning curve

## R4: AI Tip Generation

**Decision**: Backend service that analyzes user expense patterns and
generates specific, data-driven tips

**Rationale**:
- Constitution Principle 4 (Consejos de IA Honestos) requires tips based
  on real user data, never generic
- Backend has direct access to expense data for analysis
- Tips stored in `AITip` table for persistence and feedback tracking

**Pattern**:
1. Backend `AITipService` queries user expenses for patterns:
   - Month-over-month category comparison
   - Percentage of budget spent per category
   - Savings rate vs goal progress
2. Generates templates with specific numbers inserted
3. Stores generated tips with user reference
4. Frontend displays tips with useful/not useful buttons

**Alternatives considered**:
- External LLM API (OpenAI): Adds cost, latency, dependency; overkill for
  rule-based financial tips in v1
- Client-side generation: No access to historical data, violates privacy
  principle

## R5: EF Core + SQL Server Data Access

**Decision**: EF Core Code First with SQL Server, repository pattern via
service layer

**Rationale**:
- User specified SQL Server — standard choice for .NET stack
- Code First migrations allow iterative schema development
- Service layer encapsulates business logic (budget calculations,
  projection math, tip generation)

**Pattern**:
```
DbContext -> DbSets for User, Expense, SavingsGoal, SavingsEntry, AITip
Services: ExpenseService, BudgetService, GoalService, AITipService
Controllers: thin — delegate to services
```

**Alternatives considered**:
- Dapper: Faster but more manual mapping, less suited for complex
  relationships
- Cosmos DB: NoSQL would require different data modeling, overkill for
  relational financial data
