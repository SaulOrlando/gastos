# Tasks: FinanzApp Estudiantil

**Input**: Design documents from `/specs/001-student-finance-app/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Total tasks**: 97 (T001–T097, includes 6 deferred test tasks in Phase 9 and T087a performance monitoring)

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1–US5)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create backend ASP.NET Core 8 Web API project at `backend/src/FinanzApp.Api/`
- [ ] T002 [P] Create frontend Blazor WebAssembly project at `frontend/src/FinanzApp.Web/`
- [ ] T003 [P] Install Tailwind CSS v4 standalone CLI and configure MSBuild target in `frontend/src/FinanzApp.Web/FinanzApp.Web.csproj`
- [ ] T004 [P] Add NuGet packages to backend: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`
- [ ] T005 [P] Add NuGet package to frontend: `Blazor-ApexCharts`
- [ ] T006 [P] Configure Tailwind CSS theme tokens (green/purple/yellow palette, rounded typography) in `frontend/src/FinanzApp.Web/wwwroot/css/app.css`
- [ ] T007 [P] Create flat design illustration assets in `frontend/src/FinanzApp.Web/wwwroot/images/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story

**CRITICAL**: No user story work can begin until this phase is complete

- [ ] T008 Create User entity extending IdentityUser in `backend/src/FinanzApp.Api/Models/User.cs` (fields: FullName, Currency, MonthlyBudget, BudgetMonth, BudgetYear, CreatedAt)
- [ ] T009 Create AppDbContext in `backend/src/FinanzApp.Api/Data/AppDbContext.cs` with DbSets for User and UserCurrency (additional DbSets added when entities are created in later phases)
- [ ] T010 Configure Identity + EF Core in `backend/src/FinanzApp.Api/Program.cs` (AddIdentityCore, AddEntityFrameworkStores, AddApiEndpoints, MapIdentityApi)
- [ ] T011 Create and apply initial EF Core migration in `backend/src/FinanzApp.Api/Data/Migrations/`
- [ ] T012 [P] Configure cookie authentication in `backend/src/FinanzApp.Api/Program.cs` (cookie settings, session configuration)
- [ ] T013 [P] Create global error handling middleware in `backend/src/FinanzApp.Api/Middleware/ErrorHandlerMiddleware.cs`
- [ ] T014 [P] Create base API response format (error response, validation error response) in `backend/src/FinanzApp.Api/Models/ApiResponse.cs`
- [ ] T015 [P] Configure CORS and API base URL in `backend/src/FinanzApp.Api/Program.cs` — NOTE: T012 and T015 both modify Program.cs; execute sequentially or merge into a single Program.cs configuration task
- [ ] T016 Create Blazor auth infrastructure: CookieHandler, CookieAuthenticationStateProvider in `frontend/src/FinanzApp.Web/Services/`
- [ ] T017 [P] Configure HttpClient with CookieHandler in `frontend/src/FinanzApp.Web/Program.cs`
- [ ] T018 [P] Create shared layout with mobile-first navigation in `frontend/src/FinanzApp.Web/Components/Layout/MainLayout.razor`

**Checkpoint**: Foundation ready — user story implementation can begin

---

## Phase 3: User Story 1 — Landing Page & Registration (Priority: P1) — MVP

**Goal**: Student can discover the product, create an account, and access the dashboard

**Independent Test**: Visit public URL, review landing page, register with valid data, confirm redirect to empty dashboard

### Implementation for User Story 1

- [ ] T019 [P] [US1] Create RegisterRequest DTO in `frontend/src/FinanzApp.Web/Models/RegisterRequest.cs` (email, password, fullName, currency)
- [ ] T020 [P] [US1] Create AuthService in `frontend/src/FinanzApp.Web/Services/AuthService.cs` (Register, Login, Logout, GetCurrentUser)
- [ ] T021 [US1] Create Landing page in `frontend/src/FinanzApp.Web/Components/Pages/Landing.razor` (product explanation, benefits, registration CTA, mobile-first layout)
- [ ] T022 [US1] Create Registration form component in `frontend/src/FinanzApp.Web/Components/Pages/Register.razor` (name, email, password, currency dropdown, validation)
- [ ] T023 [US1] Create Login page in `frontend/src/FinanzApp.Web/Components/Pages/Login.razor` (email, password, friendly error messages)
- [ ] T024 [US1] Configure route guards: redirect unauthenticated users to Login, authenticated to Dashboard in `frontend/src/FinanzApp.Web/Components/App.razor`
- [ ] T025 [US1] Create empty Dashboard page in `frontend/src/FinanzApp.Web/Components/Pages/Dashboard.razor` (placeholder for US2)
- [ ] T026 [US1] Add registration validation (email format, password strength, duplicate email error) in `frontend/src/FinanzApp.Web/Components/Pages/Register.razor`

**Checkpoint**: Landing page and registration flow fully functional and testable

---

## Phase 4: User Story 2 — Dashboard Overview (Priority: P2)

**Goal**: Student sees financial situation at a glance: totals, charts, budget remaining

**Independent Test**: Log in with expense data, verify dashboard shows correct totals, donut chart, line chart, and budget remaining

### Implementation for User Story 2

- [ ] T027 [P] [US2] Create DashboardResponse DTO in `frontend/src/FinanzApp.Web/Models/DashboardResponse.cs` (totalExpenses, monthlyBudget, remainingBudget, categoryBreakdown, monthlyTrend, savingsGoalProgress)
- [ ] T028 [P] [US2] Create CategoryBreakdown DTO in `frontend/src/FinanzApp.Web/Models/CategoryBreakdown.cs`
- [ ] T029 [P] [US2] Create MonthlyTrend DTO in `frontend/src/FinanzApp.Web/Models/MonthlyTrend.cs`
- [ ] T030 [P] [US2] Create DashboardService in `frontend/src/FinanzApp.Web/Services/DashboardService.cs` (GET /api/dashboard, PUT /api/budget)
- [ ] T031 [P] [US2] Create DashboardController in `backend/src/FinanzApp.Api/Controllers/DashboardController.cs` (GET /api/dashboard endpoint)
- [ ] T032 [P] [US2] Create BudgetController in `backend/src/FinanzApp.Api/Controllers/BudgetController.cs` (PUT /api/budget endpoint)
- [ ] T033 [US2] Implement DashboardService.GetDashboardAsync in `backend/src/FinanzApp.Api/Services/DashboardService.cs` (aggregate expenses, calculate breakdown, monthly trend)
- [ ] T034 [US2] Implement BudgetService.SetBudgetAsync in `backend/src/FinanzApp.Api/Services/BudgetService.cs` (validate, save, recalculate)
- [ ] T035 [P] [US2] Create SummaryCard reusable component in `frontend/src/FinanzApp.Web/Components/Shared/SummaryCard.razor` (title, value, icon, mobile-first)
- [ ] T036 [P] [US2] Create DonutChart wrapper component in `frontend/src/FinanzApp.Web/Components/Shared/Charts/DonutChart.razor` (ApexCharts Donut, labels, tooltips, alt text)
- [ ] T037 [P] [US2] Create LineChart wrapper component in `frontend/src/FinanzApp.Web/Components/Shared/Charts/LineChart.razor` (ApexCharts Line, axis labels, data points, alt text)
- [ ] T038 [US2] Implement Dashboard page with summary cards, charts, and budget inline edit in `frontend/src/FinanzApp.Web/Components/Pages/Dashboard.razor`
- [ ] T039 [US2] Add empty state guidance when no expenses/budget set in `frontend/src/FinanzApp.Web/Components/Pages/Dashboard.razor`
- [ ] T040 [US2] Add loading skeleton states for dashboard in `frontend/src/FinanzApp.Web/Components/Pages/Dashboard.razor`

**Checkpoint**: Dashboard displays financial overview with charts and budget management

---

## Phase 5: User Story 3 — Expense Registration (Priority: P3)

**Goal**: Student can quickly log, edit, and delete expenses with immediate dashboard updates

**Independent Test**: Register expense, verify dashboard updates, edit expense, delete with confirmation

### Implementation for User Story 3

- [ ] T041 [P] [US3] Create Expense entity in `backend/src/FinanzApp.Api/Models/Expense.cs` (Amount, Category, Date, Note, UserId, CreatedAt, UpdatedAt)
- [ ] T042 [P] [US3] Create Category enum in `backend/src/FinanzApp.Api/Models/Category.cs` (Mensualidad, Transporte, Comida, Entretenimiento)
- [ ] T043 [P] [US3] Create ExpenseResponse DTO in `frontend/src/FinanzApp.Web/Models/ExpenseResponse.cs`
- [ ] T044 [P] [US3] Create ExpenseRequest DTO in `frontend/src/FinanzApp.Web/Models/ExpenseRequest.cs`
- [ ] T045 [P] [US3] Create ExpensesController in `backend/src/FinanzApp.Api/Controllers/ExpensesController.cs` (GET, POST, PUT, DELETE)
- [ ] T046 [US3] Implement ExpenseService in `backend/src/FinanzApp.Api/Services/ExpenseService.cs` (CRUD, validation, ownership check)
- [ ] T047 [US3] Add DbSet<Expense> to AppDbContext and configure entity (indexes, validation) in `backend/src/FinanzApp.Api/Data/AppDbContext.cs`
- [ ] T048 [US3] Create EF Core migration for Expense entity
- [ ] T049 [P] [US3] Create ExpenseService in `frontend/src/FinanzApp.Web/Services/ExpenseService.cs` (CRUD operations calling API)
- [ ] T050 [P] [US3] Create ExpenseForm reusable component in `frontend/src/FinanzApp.Web/Components/Shared/ExpenseForm.razor` (amount, category selector, date picker, note, mobile-friendly)
- [ ] T051 [P] [US3] Create ExpenseList component in `frontend/src/FinanzApp.Web/Components/Shared/ExpenseList.razor` (list with edit/delete actions)
- [ ] T052 [US3] Create Expenses page in `frontend/src/FinanzApp.Web/Components/Pages/Expenses.razor` (list + add form + edit modal)
- [ ] T053 [US3] Add expense validation (positive amount, required fields, friendly error messages) in `frontend/src/FinanzApp.Web/Components/Shared/ExpenseForm.razor`
- [ ] T054 [US3] Add delete confirmation prompt (modal or inline) in `frontend/src/FinanzApp.Web/Components/Shared/ExpenseList.razor`
- [ ] T055 [US3] Add category icons (graduation-cap, bus, utensils, gamepad) to category selector in `frontend/src/FinanzApp.Web/Components/Shared/ExpenseForm.razor`

**Checkpoint**: Full expense CRUD with immediate dashboard updates

---

## Phase 6: User Story 4 — AI-Powered Financial Tips (Priority: P4)

**Goal**: Student receives personalized, data-specific financial tips with feedback

**Independent Test**: With 2+ months of data, verify tips reference specific spending numbers; mark as useful/not useful

### Implementation for User Story 4

- [ ] T056 [P] [US4] Create AITip entity in `backend/src/FinanzApp.Api/Models/AITip.cs` (Content, GeneratedAt, IsUseful, RatedAt, UserId)
- [ ] T057 [P] [US4] Create TipsController in `backend/src/FinanzApp.Api/Controllers/TipsController.cs` (GET /api/tips, PUT /api/tips/{id}/feedback)
- [ ] T058 [US4] Implement AITipService in `backend/src/FinanzApp.Api/Services/AITipService.cs` (analyze expense patterns, generate data-specific tips, handle insufficient data)
- [ ] T059 [US4] Add DbSet<AITip> to AppDbContext and configure entity in `backend/src/FinanzApp.Api/Data/AppDbContext.cs`
- [ ] T060 [US4] Create EF Core migration for AITip entity
- [ ] T061 [P] [US4] Create AITipResponse DTO in `frontend/src/FinanzApp.Web/Models/AITipResponse.cs`
- [ ] T062 [P] [US4] Create TipsService in `frontend/src/FinanzApp.Web/Services/TipsService.cs` (GET tips, submit feedback)
- [ ] T063 [P] [US4] Create AITipCard reusable component in `frontend/src/FinanzApp.Web/Components/Shared/AITipCard.razor` (tip content, useful/not useful buttons, friendly language)
- [ ] T064 [US4] Create Tips page in `frontend/src/FinanzApp.Web/Components/Pages/Tips.razor` (list of tip cards, empty state for insufficient data)

**Checkpoint**: AI tips are data-specific, actionable, and support feedback

---

## Phase 7: User Story 5 — Savings Goals & Projection (Priority: P5)

**Goal**: Student can define savings goals, track progress, and see if they'll reach their target

**Independent Test**: Create goal, add savings, verify progress bar and projection display correctly

### Implementation for User Story 5

- [ ] T065 [P] [US5] Create SavingsGoal entity in `backend/src/FinanzApp.Api/Models/SavingsGoal.cs` (Name, TargetAmount, Deadline, IsCompleted, UserId)
- [ ] T066 [P] [US5] Create SavingsEntry entity in `backend/src/FinanzApp.Api/Models/SavingsEntry.cs` (Amount, Date, GoalId)
- [ ] T067 [P] [US5] Create GoalsController in `backend/src/FinanzApp.Api/Controllers/GoalsController.cs` (GET /api/goals, POST /api/goals, POST /api/goals/{id}/entries)
- [ ] T068 [US5] Implement GoalService in `backend/src/FinanzApp.Api/Services/GoalService.cs` (CRUD, projection calculation, completion detection)
- [ ] T069 [US5] Add DbSet<SavingsGoal> and DbSet<SavingsEntry> to AppDbContext and configure entities in `backend/src/FinanzApp.Api/Data/AppDbContext.cs`
- [ ] T070 [US5] Create EF Core migration for SavingsGoal + SavingsEntry
- [ ] T071 [P] [US5] Create GoalResponse DTO in `frontend/src/FinanzApp.Web/Models/GoalResponse.cs` (with willReach, monthlyNeeded, percentage)
- [ ] T072 [P] [US5] Create GoalService in `frontend/src/FinanzApp.Web/Services/GoalService.cs` (list goals, create goal, add savings)
- [ ] T073 [P] [US5] Create GoalCard reusable component in `frontend/src/FinanzApp.Web/Components/Shared/GoalCard.razor` (progress bar, percentage, projection, Add Savings button)
- [ ] T074 [US5] Create Goals page in `frontend/src/FinanzApp.Web/Components/Pages/Goals.razor` (list of goal cards, create goal form, empty state)
- [ ] T075 [US5] Add goal creation form (name, target amount, deadline) in `frontend/src/FinanzApp.Web/Components/Pages/Goals.razor`
- [ ] T076 [US5] Add "Add Savings" inline form on GoalCard in `frontend/src/FinanzApp.Web/Components/Shared/GoalCard.razor`
- [ ] T077 [US5] Add projection display (will reach / won't reach with suggestion) in `frontend/src/FinanzApp.Web/Components/Shared/GoalCard.razor`

**Checkpoint**: Full savings goal lifecycle with projections and feedback

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T078 [P] Create account deletion endpoint and service in `backend/src/FinanzApp.Api/Controllers/UserController.cs` (DELETE /api/user/account with cascade delete)
- [ ] T079 [P] Create profile page in `frontend/src/FinanzApp.Web/Components/Pages/Profile.razor` (view profile, delete account with confirmation)
- [ ] T079a [P] Create CurrencyDisplay helper component in `frontend/src/FinanzApp.Web/Components/Shared/CurrencyDisplay.razor` (renders amount with user's currency symbol/code, used across dashboard, expenses, goals per FR-033)
- [ ] T080 [P] Add WCAG 2.1 AA compliance: keyboard navigation, focus management, aria labels across all components in `frontend/src/FinanzApp.Web/Components/`
- [ ] T081 [P] Add alt text for all charts (DonutChart, LineChart) describing key data in `frontend/src/FinanzApp.Web/Components/Shared/Charts/`
- [ ] T082 [P] Add responsive mobile breakpoints and touch targets across all pages in `frontend/src/FinanzApp.Web/Components/Pages/`
- [ ] T083 [P] Add friendly, non-corporate tone to all UI text (Spanish) across all components in `frontend/src/FinanzApp.Web/Components/`
- [ ] T084 [P] Add plain-language explanations for financial terms on dashboard and goals pages in `frontend/src/FinanzApp.Web/Components/Pages/`
- [ ] T085 [P] Add loading skeleton states for all async pages (Dashboard, Expenses, Tips, Goals) in `frontend/src/FinanzApp.Web/Components/Pages/`
- [ ] T086 [P] Add empty state guidance for all sections (no expenses, no budget, no goals, insufficient data for tips) in `frontend/src/FinanzApp.Web/Components/Pages/`
- [ ] T087 Optimize dashboard query performance for 500+ expense records (pagination, virtualization, lazy loading, indexed queries) in `backend/src/FinanzApp.Api/Services/DashboardService.cs` and `frontend/src/FinanzApp.Web/Components/`
- [ ] T087a [P] Add performance monitoring: configure logging for response times and query performance in `backend/src/FinanzApp.Api/Program.cs` (ILoggingBuilder + request timing middleware)
- [ ] T088 Add network loss handling and retry logic for expense saving in `frontend/src/FinanzApp.Web/Services/ExpenseService.cs`
- [ ] T089 Run quickstart.md validation scenarios to verify end-to-end functionality

**Checkpoint**: All features complete, polished, and validated against quickstart

---

## Phase 9: Testing (Deferred — After All User Stories)

**Purpose**: Unit, component, and integration tests using xUnit, bUnit, and WebApplicationFactory. This phase is executed after all user stories are implemented and validated.

- [ ] T090 Set up test project structure (`backend/tests/FinanzApp.Api.Tests/`, `frontend/tests/FinanzApp.Web.Tests/`) with xUnit and bUnit packages
- [ ] T091 Write xUnit integration tests for AuthController (register, login, logout, delete account) in `backend/tests/FinanzApp.Api.Tests/Controllers/AuthControllerTests.cs`
- [ ] T092 Write xUnit integration tests for DashboardController and BudgetController (summary, charts, budget CRUD) in `backend/tests/FinanzApp.Api.Tests/Controllers/DashboardControllerTests.cs`
- [ ] T093 Write xUnit integration tests for ExpensesController (CRUD, validation, cascade updates) in `backend/tests/FinanzApp.Api.Tests/Controllers/ExpensesControllerTests.cs`
- [ ] T094 Write bUnit tests for Blazor components (SummaryCard, GoalCard, ExpenseForm, CurrencyDisplay) in `frontend/tests/FinanzApp.Web.Tests/Components/`
- [ ] T095 Write integration tests for TipService and GoalService (tip generation, projection math) in `backend/tests/FinanzApp.Api.Tests/Services/`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies — start immediately
- **Phase 2 (Foundational)**: Depends on Phase 1 — BLOCKS all user stories
- **Phase 3 (US1)**: Depends on Phase 2
- **Phase 4 (US2)**: Depends on Phase 2, integrates with US1 dashboard
- **Phase 5 (US3)**: Depends on Phase 2, updates US2 dashboard
- **Phase 6 (US4)**: Depends on Phase 2, requires US3 expense data
- **Phase 7 (US5)**: Depends on Phase 2, independent of US3/US4
- **Phase 8 (Polish)**: Depends on all user stories complete
- **Phase 9 (Tests)**: Depends on Phase 8 — runs after all features are implemented

### User Story Dependencies

- **US1 (P1)**: Can start after Phase 2 — no dependencies on other stories
- **US2 (P2)**: Can start after Phase 2 — integrates with US1 auth
- **US3 (P3)**: Can start after Phase 2 — updates US2 dashboard
- **US4 (P4)**: Can start after Phase 2 — requires US3 expense data for tips
- **US5 (P5)**: Can start after Phase 2 — independent of US3/US4

### Parallel Opportunities

**Phase 1**: T002, T003, T004, T005, T006, T007 can all run in parallel
**Phase 2**: T012, T013, T014, T015, T017, T018 can run in parallel
**US1**: T019, T020 can run in parallel
**US2**: T027, T028, T029, T030, T031, T032, T035, T036, T037 can run in parallel
**US3**: T041, T042, T043, T044, T045, T049, T050, T051 can run in parallel
**US4**: T056, T057, T061, T062, T063 can run in parallel
**US5**: T065, T066, T067, T071, T072, T073 can run in parallel
**Polish**: T078, T079, T080, T081, T082, T083, T084, T085, T086 can run in parallel

---

## Parallel Example: User Story 2

```bash
# Launch all US2 tasks together:
Task: "Create DashboardResponse DTO in frontend/src/FinanzApp.Web/Models/DashboardResponse.cs"
Task: "Create CategoryBreakdown DTO in frontend/src/FinanzApp.Web/Models/CategoryBreakdown.cs"
Task: "Create MonthlyTrend DTO in frontend/src/FinanzApp.Web/Models/MonthlyTrend.cs"
Task: "Create DashboardService in frontend/src/FinanzApp.Web/Services/DashboardService.cs"
Task: "Create DashboardController in backend/src/FinanzApp.Api/Controllers/DashboardController.cs"
Task: "Create BudgetController in backend/src/FinanzApp.Api/Controllers/BudgetController.cs"
Task: "Create SummaryCard component in frontend/src/FinanzApp.Web/Components/Shared/SummaryCard.razor"
Task: "Create DonutChart wrapper in frontend/src/FinanzApp.Web/Components/Shared/Charts/DonutChart.razor"
Task: "Create LineChart wrapper in frontend/src/FinanzApp.Web/Components/Shared/Charts/LineChart.razor"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test registration flow independently
5. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add US1 → Test → Deploy/Demo (MVP!)
3. Add US2 → Test → Deploy/Demo (Dashboard visible!)
4. Add US3 → Test → Deploy/Demo (Expenses tracked!)
5. Add US4 → Test → Deploy/Demo (AI tips active!)
6. Add US5 → Test → Deploy/Demo (Goals working!)
7. Polish → Final release
8. Testing (Phase 9) → All unit, component, and integration tests complete

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Phase 2 done:
   - Developer A: US1 (Landing + Registration)
   - Developer B: US2 (Dashboard + Charts)
   - Developer C: US3 (Expense CRUD)
3. After US1-3 complete:
   - Developer A: US4 (AI Tips)
   - Developer B: US5 (Savings Goals)
4. Final: Team collaborates on Polish phase

---

## Notes

- [P] tasks = different files, no dependencies — safe to parallelize
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- All UI text MUST use friendly, non-corporate Spanish (Constitution Principle 1)
- All charts MUST have descriptive alt text (Constitution Principle 3, FR-035)
- Currency display MUST be consistent across all areas (FR-033)
