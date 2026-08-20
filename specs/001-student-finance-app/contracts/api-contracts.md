# API Contracts: FinanzApp Estudiantil

**Date**: 2026-08-20

All endpoints return JSON. Authentication required via cookies
(`CookieHandler` on Blazor WASM client).

## Authentication

### POST /register

Register a new student account.

**Request**:
```json
{
  "email": "student@university.edu",
  "password": "SecurePass123!",
  "fullName": "María García",
  "currency": "MXN"
}
```

**Response 201**:
```json
{
  "id": "guid-string",
  "email": "student@university.edu",
  "fullName": "María García",
  "currency": "MXN"
}
```

**Response 400** (validation error):
```json
{
  "errors": {
    "Email": ["Email already exists"]
  }
}
```

---

### POST /login?useCookies=true

Authenticate and receive session cookie.

**Request**:
```json
{
  "email": "student@university.edu",
  "password": "SecurePass123!"
}
```

**Response 200**: Empty body, sets ` ASP.NETCore.Cookies` cookie.

**Response 401**:
```json
{
  "error": "Invalid email or password"
}
```

---

### POST /logout

Clear session cookie.

**Response 200**: Empty body, clears cookie.

---

## Dashboard

### GET /api/dashboard

Get current month's financial summary.

**Response 200**:
```json
{
  "totalExpenses": 4520.50,
  "monthlyBudget": 8000.00,
  "remainingBudget": 3479.50,
  "budgetMonth": 8,
  "budgetYear": 2026,
  "savingsGoalProgress": [
    {
      "goalId": 1,
      "name": "Trip fund",
      "targetAmount": 500.00,
      "savedAmount": 210.00,
      "percentage": 42.0,
      "deadline": "2026-12-31",
      "willReach": true,
      "monthlyNeeded": 96.67
    }
  ],
  "categoryBreakdown": [
    { "category": "Comida", "amount": 1850.00, "percentage": 40.9 },
    { "category": "Transporte", "amount": 1200.00, "percentage": 26.5 },
    { "category": "Mensualidad", "amount": 1000.00, "percentage": 22.1 },
    { "category": "Entretenimiento", "amount": 470.50, "percentage": 10.4 }
  ],
  "monthlyTrend": [
    { "month": "2026-06", "total": 5100.00 },
    { "month": "2026-07", "total": 4800.00 },
    { "month": "2026-08", "total": 4520.50 }
  ]
}
```

---

### PUT /api/budget

Set or update monthly budget.

**Request**:
```json
{
  "amount": 8000.00
}
```

**Response 200**:
```json
{
  "monthlyBudget": 8000.00,
  "budgetMonth": 8,
  "budgetYear": 2026
}
```

---

## Expenses

### GET /api/expenses

List expenses for current user with optional filters.

**Query Params**: `month` (int), `year` (int), `category` (string)

**Response 200**:
```json
{
  "expenses": [
    {
      "id": 1,
      "amount": 85.00,
      "category": "Comida",
      "date": "2026-08-15",
      "note": "Almuerzo con amigos",
      "createdAt": "2026-08-15T12:30:00Z"
    }
  ],
  "total": 4520.50,
  "count": 42
}
```

---

### POST /api/expenses

Create a new expense.

**Request**:
```json
{
  "amount": 85.00,
  "category": "Comida",
  "date": "2026-08-15",
  "note": "Almuerzo con amigos"
}
```

**Response 201**:
```json
{
  "id": 43,
  "amount": 85.00,
  "category": "Comida",
  "date": "2026-08-15",
  "note": "Almuerzo con amigos",
  "createdAt": "2026-08-15T12:30:00Z"
}
```

**Response 400** (validation):
```json
{
  "errors": {
    "Amount": ["Amount must be greater than 0"]
  }
}
```

---

### PUT /api/expenses/{id}

Update an existing expense.

**Request**:
```json
{
  "amount": 95.00,
  "category": "Comida",
  "date": "2026-08-15",
  "note": "Almuerzo y postre"
}
```

**Response 200**: Updated expense object.

**Response 404**: `{ "error": "Expense not found" }`

---

### DELETE /api/expenses/{id}

Delete an expense.

**Response 204**: No body.

**Response 404**: `{ "error": "Expense not found" }`

---

## Savings Goals

### GET /api/goals

List all savings goals for current user.

**Response 200**:
```json
[
  {
    "id": 1,
    "name": "Trip fund",
    "targetAmount": 500.00,
    "deadline": "2026-12-31",
    "savedAmount": 210.00,
    "percentage": 42.0,
    "willReach": true,
    "monthlyNeeded": 96.67,
    "isCompleted": false,
    "entries": [
      { "id": 1, "amount": 100.00, "date": "2026-07-15" },
      { "id": 2, "amount": 110.00, "date": "2026-08-10" }
    ]
  }
]
```

---

### POST /api/goals

Create a new savings goal.

**Request**:
```json
{
  "name": "Trip fund",
  "targetAmount": 500.00,
  "deadline": "2026-12-31"
}
```

**Response 201**: Created goal object.

**Response 400** (validation):
```json
{
  "errors": {
    "Deadline": ["Deadline must be in the future"]
  }
}
```

---

### POST /api/goals/{id}/entries

Record a savings contribution toward a goal.

**Request**:
```json
{
  "amount": 50.00
}
```

**Response 201**:
```json
{
  "id": 3,
  "amount": 50.00,
  "date": "2026-08-20",
  "goalId": 1
}
```

---

## AI Tips

### GET /api/tips

Get AI-generated tips for current user.

**Response 200**:
```json
[
  {
    "id": 1,
    "content": "This month you spent $1,850 on Comida, which is 28% more than last month. Consider meal prepping to reduce this by ~$200.",
    "generatedAt": "2026-08-20T08:00:00Z",
    "isUseful": null
  }
]
```

**Response 200** (insufficient data):
```json
[
  {
    "id": 1,
    "content": "We need at least 2 months of expense data before we can give you personalized tips. Keep tracking your expenses!",
    "generatedAt": "2026-08-20T08:00:00Z",
    "isUseful": null
  }
]
```

---

### PUT /api/tips/{id}/feedback

Mark a tip as useful or not useful.

**Request**:
```json
{
  "isUseful": true
}
```

**Response 200**: Updated tip object with `isUseful` set.

---

## User Profile

### GET /api/user/profile

Get current user's profile.

**Response 200**:
```json
{
  "id": "guid-string",
  "email": "student@university.edu",
  "fullName": "María García",
  "currency": "MXN",
  "createdAt": "2026-08-01T00:00:00Z"
}
```

---

### DELETE /api/user/account

Delete user account and all associated data.

**Response 204**: No body. Clears session cookie.

---

## Error Response Format

All errors follow a consistent format:

```json
{
  "error": "Human-readable error message"
}
```

Validation errors use:

```json
{
  "errors": {
    "FieldName": ["Error message 1", "Error message 2"]
  }
}
```
