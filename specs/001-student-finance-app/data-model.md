# Data Model: FinanzApp Estudiantil

**Date**: 2026-08-20

## Entities

### User (extends IdentityUser\<string\>)

Extends ASP.NET Core Identity user with additional financial attributes.

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | string (GUID) | PK, auto-generated | Inherited from IdentityUser |
| UserName | string | Required, unique | Email used as username |
| Email | string | Required, unique, validated | Login identifier |
| PasswordHash | string | Required | Hashed password (Identity) |
| FullName | string | Required, max 100 chars | Display name |
| Currency | string | Required, max 3 chars | ISO 4217 code (USD, MXN, EUR, COP) — locked after registration |
| MonthlyBudget | decimal(18,2) | Nullable, >= 0 | Current month's budget, null = no budget set |
| BudgetMonth | int | Nullable | Month (1-12) the budget applies to |
| BudgetYear | int | Nullable | Year the budget applies to |
| CreatedAt | DateTime | Required, auto-set | Account creation timestamp |

**Business Rules**:
- Currency is set during registration and cannot be changed (FR-033)
- MonthlyBudget resets per month — BudgetMonth/BudgetYear track which
  period the budget covers
- When MonthlyBudget is null, dashboard shows "Set Budget" prompt (SC-026)

---

### Expense

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | int | PK, auto-generated | |
| UserId | string (GUID) | FK → User.Id, required | Owner of the expense |
| Amount | decimal(18,2) | Required, > 0 | Positive number only (FR-009) |
| Category | enum | Required | Mensualidad, Transporte, Comida, Entretenimiento |
| Date | DateTime | Required, default today | When the expense occurred |
| Note | string | Nullable, max 500 chars | Optional description |
| CreatedAt | DateTime | Required, auto-set | Record creation timestamp |
| UpdatedAt | DateTime | Nullable | Last modification timestamp |

**Business Rules**:
- Amount must be positive (FR-009)
- Editable and deletable by owner (FR-030, FR-031)
- Dashboard updates immediately on create/edit/delete (FR-010)
- Indexed on (UserId, Date) for dashboard queries

---

### Category (Enum)

| Value | Display Name | Icon |
|-------|-------------|------|
| Mensualidad | Mensualidad | graduation-cap |
| Transporte | Transporte | bus |
| Comida | Comida | utensils |
| Entretenimiento | Entretenimiento | gamepad |

**Business Rules**:
- Predefined list for v1 (FR-007)
- Future extensibility for custom categories noted in spec

---

### SavingsGoal

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | int | PK, auto-generated | |
| UserId | string (GUID) | FK → User.Id, required | Owner |
| Name | string | Required, max 100 chars | Goal description (e.g., "Trip fund") |
| TargetAmount | decimal(18,2) | Required, > 0 | How much to save |
| Deadline | DateTime | Required, must be future | Target completion date |
| CreatedAt | DateTime | Required, auto-set | |
| IsCompleted | bool | Required, default false | Set when TargetAmount reached |

**Business Rules**:
- Deadline must be in the future (edge case in spec)
- Progress = sum(SavingsEntry.Amount) / TargetAmount * 100
- Projection = based on current savings rate vs time remaining
- Multiple active goals allowed per user

---

### SavingsEntry

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | int | PK, auto-generated | |
| GoalId | int | FK → SavingsGoal.Id, required | Parent goal |
| Amount | decimal(18,2) | Required, > 0 | Contribution amount |
| Date | DateTime | Required, default today | When savings were recorded |
| CreatedAt | DateTime | Required, auto-set | |

**Business Rules**:
- Recorded via "Add Savings" button on goal card (FR-028)
- Goal progress updates immediately on contribution (FR-029)
- Indexed on (GoalId, Date) for projection calculations

---

### AITip

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | int | PK, auto-generated | |
| UserId | string (GUID) | FK → User.Id, required | Target user |
| Content | string | Required, max 1000 chars | The tip text (data-specific) |
| GeneratedAt | DateTime | Required, auto-set | When tip was generated |
| IsUseful | bool | Nullable | null = not rated, true/false = rated |
| RatedAt | DateTime | Nullable | When feedback was given |

**Business Rules**:
- Tips must reference specific user data (FR-011, Principle 4)
- Insufficient data → clear message instead of generic tip (FR-012)
- Users mark as useful/not useful (FR-013)
- Feedback stored for improving future tips

---

## Relationships

```text
User (1) ──── (∗) Expense
User (1) ──── (∗) SavingsGoal
User (1) ──── (∗) AITip
SavingsGoal (1) ──── (∗) SavingsEntry
```

All child entities cascade delete with parent User (FR-019: account
deletion removes all data).

## Indexes

| Table | Index | Columns | Purpose |
|-------|-------|---------|---------|
| Expense | IX_Expense_User_Date | UserId, Date | Dashboard month queries |
| Expense | IX_Expense_User_Category | UserId, Category | Category breakdown |
| SavingsEntry | IX_SavingsEntry_Goal | GoalId, Date | Projection calculation |
| AITip | IX_AITip_User | UserId, GeneratedAt | Tip listing |

## State Transitions

### Expense

```text
[Created] → [Active] → [Edited] → [Active]
                         [Active] → [Deleted]
```

### SavingsGoal

```text
[Created] → [Active] → [Completed] (when TotalSaved >= TargetAmount)
```

No manual status toggle — completion is computed from entries.
