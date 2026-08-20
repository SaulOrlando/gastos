# MVC Contracts: FinanzApp Estudiantil

**Date**: 2026-08-20

ASP.NET Core MVC with Razor Views. Forms submit via POST, responses are
HTML views (not JSON). Authentication via ASP.NET Core Identity cookies.

## Authentication

### GET /Auth/Register

Renders registration form.

**Response**: `Views/Auth/Register.cshtml` with RegisterViewModel

---

### POST /Auth/Register

Register a new student account.

**Form Data**:
```
FullName = María García
Email = student@university.edu
Password = SecurePass123!
Currency = MXN
```

**Response 302**: Redirect to `/Dashboard`

**Response 400**: Re-renders Register.cshtml with validation errors

---

### GET /Auth/Login

Renders login form.

**Response**: `Views/Auth/Login.cshtml`

---

### POST /Auth/Login

Authenticate and set session cookie.

**Form Data**:
```
Email = student@university.edu
Password = SecurePass123!
RememberMe = false
```

**Response 302**: Redirect to `/Dashboard`

**Response 400**: Re-renders Login.cshtml with error message

---

### POST /Auth/Logout

Clear session cookie.

**Response 302**: Redirect to `/`

---

### GET /Auth/Profile

View user profile and account settings.

**Response**: `Views/Auth/Profile.cshtml`

---

### POST /Auth/DeleteAccount

Delete user account and all associated data.

**Response 302**: Redirect to `/` (logged out)

---

## Dashboard

### GET /

Landing page (public, no auth required).

**Response**: `Views/Home/Index.cshtml`

---

### GET /Dashboard

Dashboard with current month's financial summary.

**Response**: `Views/Dashboard/Index.cshtml` with DashboardViewModel containing:
- totalExpenses, monthlyBudget, remainingBudget
- categoryBreakdown (for donut chart)
- monthlyTrend (for line chart)
- savingsGoalProgress

---

### POST /Dashboard/SetBudget

Set or update monthly budget.

**Form Data**:
```
Amount = 8000.00
```

**Response 302**: Redirect to `/Dashboard` (budget updated)

---

## Expenses

### GET /Expense

List expenses for current user.

**Response**: `Views/Expense/Index.cshtml` with expense list

---

### GET /Expense/Create

Render expense creation form.

**Response**: `Views/Expense/Create.cshtml` with ExpenseFormViewModel

---

### POST /Expense/Create

Create a new expense.

**Form Data**:
```
Amount = 85.00
Category = Comida
Date = 2026-08-15
Note = Almuerzo con amigos
```

**Response 302**: Redirect to `/Expense` (expense created)

**Response 400**: Re-renders Create.cshtml with validation errors

---

### GET /Expense/Edit/{id}

Render expense edit form (pre-filled).

**Response**: `Views/Expense/Edit.cshtml` with ExpenseFormViewModel

---

### POST /Expense/Edit/{id}

Update an existing expense.

**Form Data**:
```
Amount = 95.00
Category = Comida
Date = 2026-08-15
Note = Almuerzo y postre
```

**Response 302**: Redirect to `/Expense` (expense updated)

**Response 404**: Not found view

---

### GET /Expense/Delete/{id}

Confirm expense deletion.

**Response**: `Views/Expense/Delete.cshtml` with expense details

---

### POST /Expense/Delete/{id}

Delete the expense.

**Response 302**: Redirect to `/Expense` (expense deleted)

---

## Savings Goals

### GET /Goal

List all savings goals for current user.

**Response**: `Views/Goal/Index.cshtml` with goal list + progress bars

---

### GET /Goal/Create

Render goal creation form.

**Response**: `Views/Goal/Create.cshtml`

---

### POST /Goal/Create

Create a new savings goal.

**Form Data**:
```
Name = Trip fund
TargetAmount = 500.00
Deadline = 2026-12-31
```

**Response 302**: Redirect to `/Goal` (goal created)

**Response 400**: Re-renders Create.cshtml with validation errors

---

### GET /Goal/AddSavings/{goalId}

Render savings contribution form.

**Response**: `Views/Goal/AddSavings.cshtml`

---

### POST /Goal/AddSavings/{goalId}

Record a savings contribution toward a goal.

**Form Data**:
```
Amount = 50.00
```

**Response 302**: Redirect to `/Goal` (contribution recorded)

---

## AI Tips

### GET /Tip

Get AI-generated tips for current user.

**Response**: `Views/Tip/Index.cshtml` with tip list (or empty state message)

---

### POST /Tip/Feedback/{id}

Mark a tip as useful or not useful.

**Form Data**:
```
IsUseful = true
```

**Response 302**: Redirect to `/Tip` (feedback recorded)

---

## Validation Error Handling

When form validation fails, controllers re-render the same view with
`ModelState` errors. Views display errors via `asp-validation-for` Tag Helpers.

```html
<span asp-validation-for="Amount" class="text-red-500"></span>
```

## Anti-Forgery

All POST forms include `@Html.AntiForgeryToken()` or the
`asp-antiforgery="true"` Tag Helper for CSRF protection.

## Redirect Conventions

- Successful POST → PRG (Post/Redirect/Get) pattern
- Unauthorized access → redirect to `/Auth/Login?returnUrl=...`
- Not found → return `NotFound()` view
