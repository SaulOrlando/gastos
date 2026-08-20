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

- [ ] T001 Create ASP.NET Core 8 MVC project at `src/FinanzApp.Web/`
- [ ] T002 [P] Install Tailwind CSS v4 standalone CLI and configure MSBuild target in `src/FinanzApp.Web/FinanzApp.Web.csproj`
- [ ] T003 [P] Add NuGet packages: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`
- [ ] T004 [P] Configure Tailwind CSS theme tokens (green/purple/yellow palette, rounded typography) in `src/FinanzApp.Web/wwwroot/css/app.css`
- [ ] T005 [P] Create flat design illustration assets in `src/FinanzApp.Web/wwwroot/images/`
- [ ] T006 [P] Add Chart.js CDN reference to `_Layout.cshtml` in `src/FinanzApp.Web/Views/Shared/_Layout.cshtml`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story

**CRITICAL**: No user story work can begin until this phase is complete

- [ ] T007 Create User entity extending IdentityUser in `src/FinanzApp.Web/Models/User.cs` (fields: FullName, Currency, MonthlyBudget, BudgetMonth, BudgetYear, CreatedAt)
- [ ] T008 Create AppDbContext in `src/FinanzApp.Web/Data/AppDbContext.cs` with DbSets for User (additional DbSets added when entities are created in later phases)
- [ ] T009 Configure Identity + EF Core in `src/FinanzApp.Web/Program.cs` (AddDbContext, AddIdentity, AddControllersWithViews, cookie settings)
- [ ] T010 Create and apply initial EF Core migration in `src/FinanzApp.Web/Data/Migrations/`
- [ ] T011 [P] Create shared layout with mobile-first navigation in `src/FinanzApp.Web/Views/Shared/_Layout.cshtml`
- [ ] T012 [P] Create `_ViewImports.cshtml` with Tag Helper imports in `src/FinanzApp.Web/Views/_ViewImports.cshtml`
- [ ] T013 [P] Create `_ValidationScriptsPartial.cshtml` in `src/FinanzApp.Web/Views/Shared/_ValidationScriptsPartial.cshtml`
- [ ] T014 [P] Create global error handling middleware in `src/FinanzApp.Web/Middleware/ErrorHandlerMiddleware.cs`
- [ ] T015 [P] Configure route middleware in `src/FinanzApp.Web/Program.cs` (MapControllerRoute, default route)

**Checkpoint**: Foundation ready — user story implementation can begin

---

## Phase 3: User Story 1 — Landing Page & Registration (Priority: P1) — MVP

**Goal**: Student can discover the product, create an account, and access the dashboard

**Independent Test**: Visit public URL, review landing page, register with valid data, confirm redirect to empty dashboard

### Implementation for User Story 1

- [ ] T016 [P] [US1] Create RegisterViewModel in `src/FinanzApp.Web/ViewModels/RegisterViewModel.cs` (email, password, fullName, currency)
- [ ] T017 [P] [US1] Create IAuthService + AuthService in `src/FinanzApp.Web/Services/IAuthService.cs` + `AuthService.cs` (Register, Login, Logout, GetCurrentUser)
- [ ] T018 [US1] Create AuthController in `src/FinanzApp.Web/Controllers/AuthController.cs` (Login, Register, Logout actions)
- [ ] T019 [US1] Create Landing page in `src/FinanzApp.Web/Views/Home/Index.cshtml` (product explanation, benefits, registration CTA, mobile-first layout)
- [ ] T020 [US1] Create Registration view in `src/FinanzApp.Web/Views/Auth/Register.cshtml` (name, email, password, currency dropdown, validation)
- [ ] T021 [US1] Create Login view in `src/FinanzApp.Web/Views/Auth/Login.cshtml` (email, password, friendly error messages)
- [ ] T022 [US1] Configure auth middleware: redirect unauthenticated users to Login, authenticated to Dashboard in `src/FinanzApp.Web/Program.cs`
- [ ] T023 [US1] Create empty Dashboard view in `src/FinanzApp.Web/Views/Dashboard/Index.cshtml` (placeholder for US2)
- [ ] T024 [US1] Add registration validation (email format, password strength, duplicate email error) in `src/FinanzApp.Web/Views/Auth/Register.cshtml`

**Checkpoint**: Landing page and registration flow fully functional and testable

---

## Phase 4: User Story 2 — Dashboard Overview (Priority: P2)

**Goal**: Student sees financial situation at a glance: totals, charts, budget remaining

**Independent Test**: Log in with expense data, verify dashboard shows correct totals, donut chart, line chart, and budget remaining

### Implementation for User Story 2

- [ ] T025 [P] [US2] Create DashboardViewModel in `src/FinanzApp.Web/ViewModels/DashboardViewModel.cs` (totalExpenses, monthlyBudget, remainingBudget, categoryBreakdown, monthlyTrend, savingsGoalProgress)
- [ ] T026 [P] [US2] Create IDashboardService + DashboardService in `src/FinanzApp.Web/Services/IDashboardService.cs` + `DashboardService.cs` (aggregate expenses, calculate breakdown, monthly trend)
- [ ] T027 [P] [US2] Create IBudgetService + BudgetService in `src/FinanzApp.Web/Services/IBudgetService.cs` + `BudgetService.cs` (validate, save, recalculate)
- [ ] T028 [P] [US2] Create IExpenseRepository + ExpenseRepository in `src/FinanzApp.Web/Repositories/IExpenseRepository.cs` + `ExpenseRepository.cs` (EF Core queries for dashboard)
- [ ] T029 [US2] Create DashboardController in `src/FinanzApp.Web/Controllers/DashboardController.cs` (Index for GET, SetBudget for POST)
- [ ] T030 [US2] Create Dashboard view in `src/FinanzApp.Web/Views/Dashboard/Index.cshtml` (summary cards, charts, budget inline edit)
- [ ] T031 [US2] Add Chart.js donut chart for category breakdown in `src/FinanzApp.Web/Views/Dashboard/Index.cshtml`
- [ ] T032 [US2] Add Chart.js line chart for monthly spending trends in `src/FinanzApp.Web/Views/Dashboard/Index.cshtml`
- [ ] T033 [US2] Add empty state guidance when no expenses/budget set in `src/FinanzApp.Web/Views/Dashboard/Index.cshtml`
- [ ] T034 [US2] Add loading skeleton states for dashboard in `src/FinanzApp.Web/Views/Dashboard/Index.cshtml`

**Checkpoint**: Dashboard displays financial overview with charts and budget management

---

## Phase 5: User Story 3 — Expense Registration (Priority: P3)

**Goal**: Student can quickly log, edit, and delete expenses with immediate dashboard updates

**Independent Test**: Register expense, verify dashboard updates, edit expense, delete with confirmation

### Implementation for User Story 3

- [ ] T035 [P] [US3] Create Expense entity in `src/FinanzApp.Web/Models/Expense.cs` (Amount, Category, Date, Note, UserId, CreatedAt, UpdatedAt)
- [ ] T036 [P] [US3] Create Category enum in `src/FinanzApp.Web/Models/Category.cs` (Mensualidad, Transporte, Comida, Entretenimiento)
- [ ] T037 [P] [US3] Create ExpenseFormViewModel in `src/FinanzApp.Web/ViewModels/ExpenseFormViewModel.cs`
- [ ] T038 [P] [US3] Create IExpenseService + ExpenseService in `src/FinanzApp.Web/Services/IExpenseService.cs` + `ExpenseService.cs` (CRUD, validation, ownership check)
- [ ] T039 [P] [US3] Add DbSet<Expense> to AppDbContext and configure entity (indexes, validation) in `src/FinanzApp.Web/Data/AppDbContext.cs`
- [ ] T040 [US3] Create EF Core migration for Expense entity
- [ ] T041 [US3] Create ExpenseController in `src/FinanzApp.Web/Controllers/ExpenseController.cs` (Index, Create, Edit, Delete actions)
- [ ] T042 [US3] Create Expense list view in `src/FinanzApp.Web/Views/Expense/Index.cshtml` (list with edit/delete actions)
- [ ] T043 [US3] Create Expense form view in `src/FinanzApp.Web/Views/Expense/Create.cshtml` (amount, category selector, date picker, note, mobile-friendly)
- [ ] T044 [US3] Create Expense edit view in `src/FinanzApp.Web/Views/Expense/Edit.cshtml` (pre-filled form, validation)
- [ ] T045 [US3] Create Expense delete confirmation view in `src/FinanzApp.Web/Views/Expense/Delete.cshtml`
- [ ] T046 [US3] Add expense validation (positive amount, required fields, friendly error messages) in ViewModels
- [ ] T047 [US3] Add category icons (graduation-cap, bus, utensils, gamepad) to category selector in `src/FinanzApp.Web/Views/Expense/Create.cshtml`

**Checkpoint**: Full expense CRUD with immediate dashboard updates

---

## Phase 6: User Story 4 — AI-Powered Financial Tips (Priority: P4)

**Goal**: Student receives personalized, data-specific financial tips with feedback

**Independent Test**: With 2+ months of data, verify tips reference specific spending numbers; mark as useful/not useful

### Implementation for User Story 4

- [ ] T048 [P] [US4] Create AITip entity in `src/FinanzApp.Web/Models/AITip.cs` (Content, GeneratedAt, IsUseful, RatedAt, UserId)
- [ ] T049 [P] [US4] Create ITipService + TipService in `src/FinanzApp.Web/Services/ITipService.cs` + `TipService.cs` (analyze expense patterns, generate data-specific tips, handle insufficient data)
- [ ] T050 [P] [US4] Create ITipRepository + TipRepository in `src/FinanzApp.Web/Repositories/ITipRepository.cs` + `TipRepository.cs`
- [ ] T051 [P] [US4] Add DbSet<AITip> to AppDbContext and configure entity in `src/FinanzApp.Web/Data/AppDbContext.cs`
- [ ] T052 [US4] Create EF Core migration for AITip entity
- [ ] T053 [US4] Create TipController in `src/FinanzApp.Web/Controllers/TipController.cs` (Index, Feedback actions)
- [ ] T054 [US4] Create TipViewModel in `src/FinanzApp.Web/ViewModels/TipViewModel.cs` (tip content, usefulness status)
- [ ] T055 [US4] Create Tips view in `src/FinanzApp.Web/Views/Tip/Index.cshtml` (list of tip cards, empty state for insufficient data)

**Checkpoint**: AI tips are data-specific, actionable, and support feedback

---

## Phase 7: User Story 5 — Savings Goals & Projection (Priority: P5)

**Goal**: Student can define savings goals, track progress, and see if they'll reach their target

**Independent Test**: Create goal, add savings, verify progress bar and projection display correctly

### Implementation for User Story 5

- [ ] T056 [P] [US5] Create SavingsGoal entity in `src/FinanzApp.Web/Models/SavingsGoal.cs` (Name, TargetAmount, Deadline, IsCompleted, UserId)
- [ ] T057 [P] [US5] Create SavingsEntry entity in `src/FinanzApp.Web/Models/SavingsEntry.cs` (Amount, Date, GoalId)
- [ ] T058 [P] [US5] Create IGoalService + GoalService in `src/FinanzApp.Web/Services/IGoalService.cs` + `GoalService.cs` (CRUD, projection calculation, completion detection)
- [ ] T059 [P] [US5] Create IGoalRepository + GoalRepository in `src/FinanzApp.Web/Repositories/IGoalRepository.cs` + `GoalRepository.cs`
- [ ] T060 [P] [US5] Create GoalViewModel in `src/FinanzApp.Web/ViewModels/GoalViewModel.cs` (with willReach, monthlyNeeded, percentage)
- [ ] T061 [P] [US5] Add DbSet<SavingsGoal> and DbSet<SavingsEntry> to AppDbContext and configure entities in `src/FinanzApp.Web/Data/AppDbContext.cs`
- [ ] T062 [US5] Create EF Core migration for SavingsGoal + SavingsEntry
- [ ] T063 [US5] Create GoalController in `src/FinanzApp.Web/Controllers/GoalController.cs` (Index, Create, AddSavings actions)
- [ ] T064 [US5] Create Goals list view in `src/FinanzApp.Web/Views/Goal/Index.cshtml` (list of goal cards, empty state)
- [ ] T065 [US5] Create Goal creation form in `src/FinanzApp.Web/Views/Goal/Create.cshtml` (name, target amount, deadline)
- [ ] T066 [US5] Create Add Savings form in `src/FinanzApp.Web/Views/Goal/AddSavings.cshtml` (amount, date defaults to today)
- [ ] T067 [US5] Add progress bar, percentage, and projection display in `src/FinanzApp.Web/Views/Goal/Index.cshtml`
- [ ] T068 [US5] Add projection display (will reach / won't reach with suggestion) in `src/FinanzApp.Web/Views/Goal/Index.cshtml`

**Checkpoint**: Full savings goal lifecycle with projections and feedback

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T069 [P] Create account deletion action in `src/FinanzApp.Web/Controllers/AuthController.cs` (DeleteAccount POST with cascade delete)
- [ ] T070 [P] Create profile/account view in `src/FinanzApp.Web/Views/Auth/Profile.cshtml` (view profile, delete account with confirmation)
- [ ] T070a [P] Create `_CurrencyHelper.cshtml` partial view in `src/FinanzApp.Web/Views/Shared/_CurrencyHelper.cshtml` (renders amount with user's currency symbol/code, used across dashboard, expenses, goals per FR-033)
- [ ] T071 [P] Add WCAG 2.1 AA compliance: keyboard navigation, focus management, aria labels across all Views in `src/FinanzApp.Web/Views/`
- [ ] T072 [P] Add alt text for all charts (donut, line) describing key data in `src/FinanzApp.Web/Views/Dashboard/Index.cshtml`
- [ ] T073 [P] Add responsive mobile breakpoints and touch targets across all Views in `src/FinanzApp.Web/Views/`
- [ ] T074 [P] Add friendly, non-corporate tone to all UI text (Spanish) across all Views in `src/FinanzApp.Web/Views/`
- [ ] T075 [P] Add plain-language explanations for financial terms on dashboard and goals pages in `src/FinanzApp.Web/Views/`
- [ ] T076 [P] Add empty state guidance for all sections (no expenses, no budget, no goals, insufficient data for tips) in `src/FinanzApp.Web/Views/`
- [ ] T077 Optimize dashboard query performance for 500+ expense records (indexed queries, projection) in `src/FinanzApp.Web/Services/DashboardService.cs`
- [ ] T077a [P] Add performance monitoring: configure logging for response times in `src/FinanzApp.Web/Program.cs`
- [ ] T078 Run quickstart.md validation scenarios to verify end-to-end functionality

**Checkpoint**: All features complete, polished, and validated against quickstart

---

## Phase 9: Testing (Deferred — After All User Stories)

**Purpose**: Unit and integration tests using xUnit. This phase is executed after all user stories are implemented and validated.

- [ ] T079 Set up test project structure at `tests/FinanzApp.Web.Tests/` with xUnit packages
- [ ] T080 Write unit tests for AuthService (register, login, logout, delete account) in `tests/FinanzApp.Web.Tests/Services/AuthServiceTests.cs`
- [ ] T081 Write unit tests for DashboardService and BudgetService (summary, charts, budget CRUD) in `tests/FinanzApp.Web.Tests/Services/DashboardServiceTests.cs`
- [ ] T082 Write unit tests for ExpenseService (CRUD, validation, ownership check) in `tests/FinanzApp.Web.Tests/Services/ExpenseServiceTests.cs`
- [ ] T083 Write unit tests for TipService and GoalService (tip generation, projection math) in `tests/FinanzApp.Web.Tests/Services/TipServiceTests.cs`
- [ ] T084 Write integration tests for Controllers (Auth, Dashboard, Expense, Goal, Tip) in `tests/FinanzApp.Web.Tests/Controllers/`

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

**Phase 1**: T002, T003, T004, T005, T006 can all run in parallel
**Phase 2**: T011, T012, T013, T014, T015 can run in parallel
**US1**: T016, T017 can run in parallel
**US2**: T025, T026, T027, T028 can run in parallel
**US3**: T035, T036, T037, T038, T039 can run in parallel
**US4**: T048, T049, T050, T051 can run in parallel
**US5**: T056, T057, T058, T059, T060, T061 can run in parallel
**Polish**: T069, T070, T070a, T071, T072, T073, T074, T075, T076 can run in parallel

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
8. Testing (Phase 9) → All tests complete

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
