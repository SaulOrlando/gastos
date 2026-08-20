# Feature Specification: FinanzApp Estudiantil

**Feature Branch**: `001-student-finance-app`

**Created**: 2026-08-20

**Status**: Draft

**Input**: User description: "Construye FinanzApp Estudiantil, una app web donde estudiantes universitarios registran sus gastos (mensualidad, transporte, comida, entretenimiento) y reciben consejos personalizados generados por IA para ahorrar mejor."

## Clarifications

### Session 2026-08-20

- Q: How does a student set or change their monthly budget? → A: Inline edit on dashboard — tap "Set Budget" to enter amount for the month.
- Q: How does a student record savings contributions toward a goal? → A: "Add Savings" button on each goal card — enter amount, auto-dates to today.
- Q: Can students edit or delete expenses after recording them? → A: Edit and delete — students can modify amount/category/note or remove entirely.
- Q: Which currency should the app use, and how is it selected? → A: Select during registration — dropdown with common currencies, locked after.
- Q: Should the app meet basic web accessibility standards? → A: WCAG 2.1 AA — screen reader support, 4.5:1 contrast, keyboard navigation.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Landing Page and Registration (Priority: P1)

A university student discovers FinanzApp through a link or search. They see a
clear landing page explaining what the app does, its benefits, and a prominent
call-to-action to create an account. The student fills out a short registration
form (name, email, password, currency) and is immediately taken to their empty
dashboard.

**Why this priority**: Without the landing page and registration flow, no users
can access the app. This is the entry point for the entire experience and must
be polished to convert visitors into users.

**Independent Test**: Can be fully tested by visiting the public URL, reviewing
the landing page content, completing registration, and confirming redirect to
the dashboard. Delivers value by enabling user acquisition.

**Acceptance Scenarios**:

1. **Given** a visitor is on the landing page, **When** they scroll through the
   page, **Then** they see a clear explanation of the app's purpose, key
   benefits (track expenses, get AI tips, reach savings goals), and a
   registration form.

2. **Given** a visitor is on the landing page, **When** they fill out the
   registration form with valid data (name, email, password, currency), **Then**
   a new account is created and they are redirected to the dashboard.

3. **Given** a visitor is on the landing page, **When** they submit the
   registration form with an email that already exists, **Then** they see a
   friendly error message suggesting they log in instead.

4. **Given** a visitor is on the landing page, **When** they view the page on a
   mobile device, **Then** all content is readable and the registration form is
   easy to fill out on a small screen.

---

### User Story 2 - Dashboard Overview (Priority: P2)

A registered student opens the app and immediately sees their financial
situation for the current month: total expenses, remaining budget versus a
defined monthly budget, and progress toward a savings goal. Two charts are
prominent: a pie/donut chart showing expense distribution by category and a
line chart showing spending trends over time.

**Why this priority**: The dashboard is the core value proposition. It answers
the question "Where am I financially?" in seconds. This is what keeps users
coming back.

**Independent Test**: Can be tested by logging in with an account that has
expense data and verifying all dashboard elements (totals, charts, budget
remaining) display correctly. Delivers value by providing financial visibility.

**Acceptance Scenarios**:

1. **Given** a student is logged in, **When** they navigate to the dashboard,
   **Then** they see the current month's total expenses, remaining budget (total
   budget minus expenses), and savings goal progress.

2. **Given** a student has expenses in multiple categories, **When** they view
   the dashboard, **Then** a pie/donut chart displays the percentage breakdown
   by category with clear labels and colors.

3. **Given** a student has expenses spanning multiple months, **When** they view
   the dashboard, **Then** a line chart shows monthly spending trends with
   clearly labeled axes and data points.

4. **Given** a student has no expenses yet, **When** they view the dashboard,
   **Then** they see helpful empty states with guidance on how to start tracking.

5. **Given** a student is on the dashboard, **When** they view it on a mobile
   device, **Then** charts and numbers are legible and the layout adapts
   responsively without horizontal scrolling.

6. **Given** a student has no budget set, **When** they view the dashboard,
   **Then** they see a "Set Budget" prompt that allows them to enter a monthly
   budget amount inline.

7. **Given** a student taps "Set Budget," **When** they enter an amount and
   confirm, **Then** the budget is saved and the remaining budget updates
   immediately.

8. **Given** a student has an existing budget, **When** they tap the budget
   display, **Then** they can edit the amount inline and the dashboard reflects
   the change.

---

### User Story 3 - Register a New Expense (Priority: P3)

A student finishes lunch and quickly wants to log what they spent. They tap a
"Register Expense" button, enter the amount, select a category from a simple
list (mensualidad, transporte, comida, entretenimiento), optionally add a date
and a short note, and save. The expense appears immediately in their dashboard.

**Why this priority**: Expense registration is the primary action users perform.
If it's not fast and frictionless, students won't bother logging expenses and
the app loses its data foundation.

**Independent Test**: Can be tested by tapping "Register Expense," filling the
form, saving, and confirming the expense shows in the dashboard totals and
charts. Delivers value by enabling data input.

**Acceptance Scenarios**:

1. **Given** a student is on any screen, **When** they tap "Register Expense,"
   **Then** a simple form appears with fields for amount, category (dropdown or
   selector), date (defaults to today), and an optional note.

2. **Given** a student fills in amount and selects a category, **When** they tap
   "Save," **Then** the expense is recorded and the dashboard updates to reflect
   the new total.

3. **Given** a student submits the form without an amount, **When** they tap
   "Save," **Then** they see a clear validation message indicating the amount
   is required.

4. **Given** a student adds a note, **When** they save the expense, **Then** the
   note is stored and visible when reviewing expense details.

5. **Given** a student is on the expense form, **When** they view it on mobile,
   **Then** the form is easy to fill out with large touch targets and a numeric
   keyboard for the amount.

6. **Given** a student views their expense list, **When** they tap on an
   existing expense, **Then** they can edit the amount, category, date, or note.

7. **Given** a student is editing an expense, **When** they save changes, **Then**
   the dashboard totals and charts update immediately.

8. **Given** a student views their expense list, **When** they swipe or tap a
   delete action on an expense, **Then** a confirmation prompt appears before
   removal.

9. **Given** a student confirms deletion, **When** the expense is removed,
   **Then** the dashboard totals and charts update immediately.

10. **Given** a student navigates the app with a screen reader, **When** they
    encounter charts or visual data, **Then** descriptive alt text conveys the
    key information (top category, total amount, trend direction).

---

### User Story 4 - AI-Powered Financial Tips (Priority: P4)

A student opens the tips section and sees personalized recommendations based on
their actual spending patterns. For example, "You spent 40% more on
entretenimiento this month compared to last month — consider setting a weekly
limit of $X." Each tip can be marked as useful or not useful, helping improve
future recommendations.

**Why this priority**: AI tips are the key differentiator that transforms raw
data into actionable insights. This feature delivers the "save better" promise
of the product.

**Independent Test**: Can be tested by reviewing the tips section with an
account that has sufficient expense history (2+ months) and verifying tips are
specific to the user's data, not generic. Delivers value by providing
personalized guidance.

**Acceptance Scenarios**:

1. **Given** a student has at least one month of expense data, **When** they
   open the tips section, **Then** they see at least one tip that references
   specific numbers from their spending (e.g., "You spent $X on Y this month").

2. **Given** a student has insufficient data for reliable advice, **When** they
   open the tips section, **Then** the app clearly states it needs more data
   before providing personalized tips.

3. **Given** a student sees a tip, **When** they mark it as useful, **Then** the
   tip is visually marked as acknowledged and this feedback is recorded.

4. **Given** a student sees a tip, **When** they mark it as not useful, **Then**
   the tip is visually marked and this feedback is recorded for improvement.

5. **Given** a student is on the tips section, **When** they view it on mobile,
   **Then** each tip is readable in a card format with clear actions.

---

### User Story 5 - Savings Goals and Projection (Priority: P5)

A student sets a savings goal (e.g., "Save $500 for a trip by December") and
the app tracks their progress. The goal section shows how much has been saved,
how much remains, and a projection of whether they'll reach the goal based on
their current savings rate. If the projection is negative, the app suggests
adjustments.

**Why this priority**: Savings goals add motivation and long-term engagement.
They transform expense tracking from a passive activity into an active pursuit.
This is valuable but depends on having expense data first.

**Independent Test**: Can be tested by creating a savings goal, adding some
savings data, and verifying the progress bar and projection calculations
display correctly. Delivers value by motivating financial discipline.

**Acceptance Scenarios**:

1. **Given** a student is on the savings goals section, **When** they create a
   new goal with a target amount and deadline, **Then** the goal appears with a
   progress indicator showing 0% and the target amount.

2. **Given** a student has a savings goal and some savings history, **When**
   they view the goal, **Then** they see a progress bar, the percentage saved,
   and a projection of whether they'll reach the goal by the deadline.

3. **Given** a student's projection shows they won't reach their goal, **When**
   they view the projection, **Then** the app shows a specific suggestion (e.g.,
   "You need to save $X more per month to reach your goal").

4. **Given** a student has multiple savings goals, **When** they view the goals
   section, **Then** all goals are listed with individual progress indicators.

5. **Given** a student is on the savings goals section, **When** they view it
   on mobile, **Then** progress bars and projections are clearly visible and
   easy to understand.

6. **Given** a student has an active savings goal, **When** they tap "Add
   Savings" on the goal card, **Then** a simple form appears to enter an amount
   (date defaults to today).

7. **Given** a student enters a savings amount and confirms, **When** the
   contribution is saved, **Then** the goal's progress bar and projection update
   immediately.

---

### Edge Cases

- What happens when a student tries to register with an invalid email format?
- What happens when a student enters a negative amount for an expense?
- What happens when a student's monthly budget is set to $0?
- What happens when the tip generation service fails or has insufficient data
  to generate a recommendation?
- What happens when a student has expenses but no savings goal defined?
- What happens when a student tries to set a savings goal with a past deadline?
- What happens when a student deletes their account — are all financial
  records permanently removed?
- What happens when network connectivity is lost while saving an expense?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow visitors to view a public landing page
  describing the product without authentication.
- **FR-002**: System MUST allow visitors to create an account with name, email,
  password, and currency selection from a predefined list.
- **FR-003**: System MUST authenticate users via email and password to access
  the dashboard and features.
- **FR-033**: System MUST lock currency selection after registration and display
  the chosen currency symbol throughout the app.
- **FR-004**: System MUST display a dashboard with current month's total
  expenses, remaining budget, and savings goal progress.
- **FR-005**: System MUST display a pie/donut chart showing expense
  distribution by category with percentage labels.
- **FR-006**: System MUST display a line chart showing monthly spending trends
  over time.
- **FR-007**: System MUST provide a quick expense registration form with fields
  for amount, category (from predefined list: mensualidad, transporte, comida,
  entretenimiento), date, and optional note.
- **FR-008**: System MUST default the expense date to the current date when
  opening the form.
- **FR-009**: System MUST validate that expense amount is a positive number
  before saving.
- **FR-010**: System MUST update the dashboard immediately after any expense
  change (create, edit, or delete).
- **FR-011**: System MUST generate AI tips that reference specific user data
  (actual spending numbers, categories, trends).
- **FR-012**: System MUST indicate when insufficient data exists for reliable
  AI recommendations.
- **FR-013**: System MUST allow users to mark AI tips as useful or not useful.
- **FR-014**: System MUST allow users to define savings goals with a target
  amount and deadline.
- **FR-015**: System MUST display savings goal progress as a percentage and
  remaining amount.
- **FR-016**: System MUST calculate and display a projection of whether the
  savings goal will be reached based on current savings rate.
- **FR-017**: System MUST provide specific adjustment suggestions when a savings
  goal projection is off-track.
- **FR-018**: System MUST persist all user data (expenses, goals, preferences)
  across sessions.
- **FR-019**: System MUST allow users to delete their account and all associated
  data permanently.
- **FR-020**: System MUST encrypt all financial data in storage and during
  transmission.
- **FR-021**: System MUST display empty states with helpful guidance when no
  data exists.
- **FR-022**: System MUST ensure all screens and components are responsive and
  functional on mobile devices.
- **FR-023**: System MUST load the dashboard and charts within 3 seconds
  (per SC-003) even with large volumes of expense records.
- **FR-024**: System MUST handle tip generation failures gracefully (e.g.
  database unavailable, insufficient data) with a user-friendly fallback
  message.
- **FR-025**: System MUST handle network loss during expense saving with local
  indication and retry capability.
- **FR-026**: System MUST provide an inline "Set Budget" control on the dashboard
  allowing students to enter or edit their monthly budget amount.
- **FR-027**: System MUST update remaining budget calculations immediately after
  budget is set or modified.
- **FR-028**: System MUST provide an "Add Savings" button on each active goal
  card allowing students to record a contribution with an amount (date defaults
  to today).
- **FR-029**: System MUST update goal progress and projection immediately after
  a savings contribution is recorded.
- **FR-030**: System MUST allow students to edit an existing expense's amount,
  category, date, or note.
- **FR-031**: System MUST allow students to delete an existing expense with a
  confirmation prompt before removal.
- **FR-034**: System MUST meet WCAG 2.1 AA accessibility standards including
  screen reader support, minimum 4.5:1 color contrast ratio, and full
  keyboard navigability.
- **FR-035**: System MUST provide descriptive alt text for all charts and visual
  elements to ensure screen reader users can interpret financial data.

### Key Entities

- **User**: Represents a registered student. Key attributes: name, email,
  password (hashed), currency, monthly budget, created date.
- **Expense**: Represents a single spending record. Key attributes: amount,
  category, date, note, user reference, created date.
- **Category**: Predefined expense classification. Values: mensualidad,
  transporte, comida, entretenimiento. Future extensibility for custom
  categories.
- **SavingsGoal**: Represents a financial objective. Key attributes: target
  amount, deadline, creation date, user reference.
- **SavingsEntry**: Records contributions toward a savings goal. Key
  attributes: amount, date, goal reference.
- **AITip**: Represents a generated recommendation. Key attributes: content,
  generated date, usefulness feedback, user reference.

Note: Monthly budget is stored as attributes on the User entity
(MonthlyBudget, BudgetMonth, BudgetYear) per data-model.md. No separate
MonthlyBudget entity is needed.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A new student can complete registration and land on the dashboard
  in under 2 minutes.
- **SC-002**: A student can register a new expense in under 30 seconds from
  tapping "Register Expense" to seeing confirmation.
- **SC-003**: The dashboard loads and renders all charts within 3 seconds on a
  standard mobile connection.
- **SC-004**: At least 80% of students with 2+ months of data rate AI tips as
  relevant and specific (not generic).
- **SC-005**: Students can understand their monthly financial situation (total
  spent, budget remaining, top category) within 5 seconds of viewing the
  dashboard.
- **SC-006**: The app supports at least 500 expense records per user without
  noticeable performance degradation on the dashboard.
- **SC-007**: 90% of students complete the expense registration form
  successfully on first attempt without errors.
- **SC-008**: Savings goal projections accurately reflect the user's savings
  rate based on their historical data.
- **SC-009**: The app passes automated WCAG 2.1 AA compliance checks with zero
  critical violations.

## Assumptions

- Students have access to a modern web browser on mobile or desktop.
- Students are comfortable with basic web interactions (filling forms, tapping
  buttons).
- The predefined expense categories (mensualidad, transporte, comida,
  entretenimiento) cover the majority of student spending patterns for v1.
- Personalized tips are generated by a backend service that analyzes user
  expense patterns using rule-based logic (no external AI/LLM API dependency;
  see research.md R4).
- Students will input accurate expense data; the app does not verify receipts
  or transactions.
- Monthly budget is set once per month by the user and applies to all expenses
  in that period.
- The app operates in a single currency determined at account creation.
- Students are motivated by visual progress indicators and projections.
- Privacy compliance requirements follow standard data protection regulations
  applicable to educational tools.
