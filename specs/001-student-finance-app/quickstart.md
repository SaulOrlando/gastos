# Quickstart Validation Guide: FinanzApp Estudiantil

**Date**: 2026-08-20

## Prerequisites

- .NET 8 SDK installed
- SQL Server (LocalDB or full instance)
- Tailwind CSS v4 standalone CLI (downloaded during setup)

## Setup

```bash
# Clone and restore
git clone <repo-url> && cd gastos

# MVC project setup
cd src/FinanzApp.Web
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Validation Scenarios

### V1: Registration Flow (P1)

**Steps**:
1. Run the app: `dotnet run` in `src/FinanzApp.Web`
2. Navigate to `https://localhost:5001` (or configured port)
3. Verify landing page shows product explanation, benefits, registration form
4. Fill registration form (name, email, password, currency)
5. Submit and verify redirect to empty dashboard

**Expected**: Account created, user lands on dashboard with empty states
and "Set Budget" prompt visible.

---

### V2: Dashboard + Budget Setup (P2)

**Steps**:
1. Log in with the registered account
2. Verify dashboard shows: total expenses = $0, "Set Budget" prompt
3. Tap "Set Budget" and enter amount (e.g., $8,000)
4. Confirm budget is saved and dashboard updates

**Expected**: Remaining budget shows the full budget amount. Empty state
messages guide the user to add expenses.

---

### V3: Expense Registration (P3)

**Steps**:
1. Tap "Register Expense" on dashboard
2. Fill: amount = $85, category = Comida, date = today, note = "Lunch"
3. Save and verify dashboard updates
4. Add 3 more expenses in different categories
5. Verify donut chart shows category breakdown
6. Verify line chart shows current month

**Expected**: Charts render with correct data. Totals update immediately.

---

### V4: Expense Edit/Delete (P3)

**Steps**:
1. Tap on an existing expense in the list
2. Change amount from $85 to $95, save
3. Verify dashboard total updates
4. Tap delete on another expense
5. Confirm deletion prompt appears
6. Confirm deletion

**Expected**: Dashboard reflects changes immediately. Charts recalculate.

---

### V5: AI Tips (P4)

**Steps**:
1. Add expenses for at least 2 different months
2. Navigate to Tips section
3. Verify tip references specific spending data (e.g., "You spent $X on Y")
4. Mark a tip as useful
5. Verify feedback is recorded

**Expected**: Tips are specific to user data, not generic. If insufficient
data, a clear message is shown.

---

### V6: Savings Goals (P5)

**Steps**:
1. Navigate to Goals section
2. Create goal: "Trip fund", target $500, deadline Dec 2026
3. Verify progress bar shows 0%
4. Tap "Add Savings", enter $100
5. Verify progress updates to 20%
6. Verify projection shows whether goal will be reached

**Expected**: Progress bar, percentage, and projection display correctly.
Off-track goals show specific adjustment suggestions.

---

### V7: Mobile Responsiveness

**Steps**:
1. Open app on mobile device or resize browser to 375px width
2. Verify landing page is readable and form is usable
3. Verify dashboard charts don't overflow horizontally
4. Verify expense form has large touch targets and numeric keyboard
5. Verify all navigation works on mobile

**Expected**: All screens are functional and readable on mobile. No
horizontal scrolling on dashboard.

---

### V8: Accessibility

**Steps**:
1. Navigate the app using only keyboard (Tab, Enter, Escape)
2. Run automated WCAG 2.1 AA checker (e.g., axe DevTools)
3. Verify screen reader can interpret chart data via alt text
4. Verify color contrast meets 4.5:1 ratio

**Expected**: Zero critical WCAG violations. All interactive elements
are keyboard-accessible. Charts have descriptive alt text.

---

### V9: Account Deletion

**Steps**:
1. Navigate to profile/account settings
2. Initiate account deletion
3. Confirm deletion
4. Attempt to log in with deleted credentials

**Expected**: Account and all data permanently removed. Login fails
with generic error (no user enumeration).
