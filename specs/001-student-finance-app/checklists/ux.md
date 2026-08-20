# UX Requirements Quality Checklist: FinanzApp Estudiantil

**Purpose**: Validate that dashboard, expense registration, and savings goals
requirements are clear and usable for someone without financial knowledge
(constitution Principle 1 + Principle 3)
**Created**: 2026-08-20
**Feature**: [spec.md](../spec.md)

## Dashboard — Requirement Completeness

- [ ] CHK001 Are all dashboard summary elements (total expenses, remaining budget,
  savings goal progress) explicitly defined with their visual hierarchy and
  positioning? [Completeness, Spec §FR-004]
- [ ] CHK002 Is the donut chart's "percentage breakdown by category" requirement
  complete — does it specify what happens when only one category exists, or when
  categories are tied? [Completeness, Spec §FR-005]
- [ ] CHK003 Is the line chart's "monthly spending trends" requirement complete —
  does it specify the time range (how many months displayed), axis labels, and
  data point format? [Completeness, Spec §FR-006]
- [ ] CHK004 Are empty state requirements defined for each dashboard element
  individually (empty expenses, no budget set, no savings goal), or only as a
  generic "helpful guidance"? [Completeness, Spec §FR-021, §User Story 2]
- [ ] CHK005 Is the "Set Budget" inline control requirement complete — does it
  specify the input format (numeric field, slider, etc.), validation rules, and
  how the dashboard reflects the change visually? [Completeness, Spec §FR-026]
- [ ] CHK006 Are loading state requirements defined for the dashboard when data
  is being fetched (skeleton screens, spinners, or progressive loading)?
  [Gap, Spec §FR-023]

## Dashboard — Requirement Clarity

- [ ] CHK007 Is "remaining budget" defined in plain language that a non-financial
  user would understand, or does it assume knowledge of budget terminology?
  [Clarity, Spec §FR-004, Constitution Principle 1]
- [ ] CHK008 Is "prominent" quantified for the two dashboard charts — does the
  spec define minimum size, positioning, or visual weight relative to other
  elements? [Clarity, Spec §User Story 2]
- [ ] CHK009 Is "clear labels and colors" quantified for the donut chart — are
  the exact labels, color assignments, and tooltip content specified?
  [Clarity, Spec §FR-005]
- [ ] CHK010 Is "clearly labeled axes" quantified for the line chart — are axis
  labels, date format, currency display, and grid behavior specified?
  [Clarity, Spec §FR-006]
- [ ] CHK011 Is the term "remaining budget" interpreted consistently between
  §FR-004 (dashboard display) and §FR-027 (budget update behavior)?
  [Consistency, Spec §FR-004, §FR-027]
- [ ] CHK012 Is the budget month/year logic explained — when a student sets a
  budget, is it clear that it applies only to the current month and resets
  next month? [Clarity, Spec §FR-026]

## Dashboard — Non-Financial User Lens

- [ ] CHK013 Are financial terms on the dashboard (budget, expenses, savings
  goal) accompanied by plain-language explanations or tooltips for users
  without financial background? [Gap, Constitution Principle 1, §FR-004]
- [ ] CHK014 Is the donut chart's category breakdown explained with a brief
  description (e.g., "This shows where your money went this month")?
  [Gap, Constitution Principle 3, §FR-005]
- [ ] CHK015 Is the line chart's trend explained in accessible terms (e.g.,
  "Your spending this month compared to previous months") rather than
  assuming the user understands trend interpretation? [Gap, Constitution
  Principle 3, §FR-006]

## Dashboard — Scenario Coverage

- [ ] CHK016 Are requirements defined for what the dashboard shows when a student
  has expenses but no budget set AND no savings goal? [Coverage, Edge Case]
- [ ] CHK017 Are requirements defined for what the dashboard shows when a student
  has a budget set but zero expenses for the current month? [Coverage, Edge Case]
- [ ] CHK018 Are requirements defined for the visual feedback when a budget is
  set or edited — does the remaining budget animate or update instantly?
  [Coverage, Spec §FR-027]

## Expense Registration — Requirement Completeness

- [ ] CHK019 Is the expense form requirement complete — does it specify all
  fields (amount, category, date, note), their input types, and validation
  rules for each? [Completeness, Spec §FR-007]
- [ ] CHK020 Is the category selector requirement complete — does it specify
  whether it's a dropdown, radio buttons, or icon grid, and whether categories
  have icons or descriptions? [Completeness, Spec §FR-007]
- [ ] CHK021 Is the date field requirement complete — does it specify the date
  picker format, default value behavior, and whether past/future dates are
  allowed? [Completeness, Spec §FR-008]
- [ ] CHK022 Is the note field requirement complete — does it specify max length,
  placeholder text, and whether it's single-line or multi-line?
  [Completeness, Spec §FR-007]
- [ ] CHK023 Is the expense edit flow requirement complete — does it specify
  whether the same form is reused or a different layout is used for editing?
  [Completeness, Spec §FR-030]
- [ ] CHK024 Is the expense deletion confirmation requirement complete — does
  it specify the confirmation UI (modal, inline, toast) and the text shown
  to the user? [Completeness, Spec §FR-031]

## Expense Registration — Requirement Clarity

- [ ] CHK025 Is "quick expense registration" quantified — is there a target
  time or number of taps from entry to saved expense? [Clarity, Spec §FR-007,
  §SC-002]
- [ ] CHK026 Is "large touch targets" quantified with specific minimum dimensions
  for mobile expense form elements? [Clarity, Spec §User Story 3]
- [ ] CHK027 Is "numeric keyboard for the amount" specified as a requirement or
  an implementation detail — is the mobile input type explicitly required?
  [Clarity, Spec §User Story 3]
- [ ] CHK028 Is the validation message for missing amount specified in plain
  language (e.g., "Ingresa cuánto gastaste" vs "Amount is required")?
  [Clarity, Spec §FR-009, Constitution Principle 1]

## Expense Registration — Non-Financial User Lens

- [ ] CHK029 Are category names (mensualidad, transporte, comida, entretenimiento)
  accompanied by icons or brief descriptions so a non-financial user
  understands what each means? [Gap, Constitution Principle 1, §FR-007]
- [ ] CHK030 Is the expense form's tone of language specified as friendly and
  non-corporate (e.g., "¿En qué gastaste?" vs "Registrar gasto")?
  [Gap, Constitution Principle 1]
- [ ] CHK031 Is the success feedback after saving an expense written in
  accessible, motivating language rather than technical confirmation?
  [Gap, Constitution Principle 1, §FR-010]

## Expense Registration — Scenario Coverage

- [ ] CHK032 Are requirements defined for what happens when a student tries to
  save an expense with a negative amount — does the error message explain
  why in plain language? [Coverage, Edge Case, §FR-009]
- [ ] CHK033 Are requirements defined for the visual state of the form after
  saving (does it clear, stay filled, or close)?
  [Coverage, Gap, §FR-010]
- [ ] CHK034 Are requirements defined for expense list display — how expenses
  are ordered, grouped, and paginated on mobile?
  [Gap, Spec §FR-030]

## Savings Goals — Requirement Completeness

- [ ] CHK035 Is the savings goal creation form requirement complete — does it
  specify all fields (name, target amount, deadline), input types, and
  validation rules? [Completeness, Spec §FR-014]
- [ ] CHK036 Is the goal card requirement complete — does it specify what
  information is displayed on each card (name, progress bar, percentage,
  amount remaining, deadline)? [Completeness, Spec §FR-015]
- [ ] CHK037 Is the "Add Savings" button requirement complete — does it specify
  the form layout, input validation, and confirmation feedback?
  [Completeness, Spec §FR-028]
- [ ] CHK038 Is the projection display requirement complete — does it specify
  the exact wording shown when the goal will be reached vs. when it won't?
  [Completeness, Spec §FR-016, §FR-017]

## Savings Goals — Requirement Clarity

- [ ] CHK039 Is "savings goal progress" defined in plain language — does the
  spec explain what percentage and remaining amount mean in accessible terms?
  [Clarity, Spec §FR-015, Constitution Principle 1]
- [ ] CHK040 Is the projection calculation explanation accessible — does the
  spec define what "based on current savings rate" means in terms a
  non-financial user would understand? [Clarity, Spec §FR-016]
- [ ] CHK041 Is the adjustment suggestion language specified — when a goal is
  off-track, is the suggestion phrased as a friendly recommendation rather
  than a financial directive? [Clarity, Spec §FR-017, Constitution Principle 1]
- [ ] CHK042 Is the goal deadline displayed in user-friendly format (e.g.,
  "diciembre 2026" vs "2026-12-31")? [Clarity, Spec §FR-014]

## Savings Goals — Non-Financial User Lens

- [ ] CHK043 Is the concept of a "savings goal" explained to the user before
  they create one (e.g., a brief intro on the goals page)?
  [Gap, Constitution Principle 1, §FR-014]
- [ ] CHK044 Is the progress bar explained with a tooltip or label (e.g.,
  "Has ahorrado $210 de $500") rather than just showing a percentage?
  [Gap, Constitution Principle 3, §FR-015]
- [ ] CHK045 Is the "Add Savings" action described in accessible language
  (e.g., "Registrar ahorro" vs "Add Savings Entry")?
  [Gap, Constitution Principle 1, §FR-028]

## Savings Goals — Scenario Coverage

- [ ] CHK046 Are requirements defined for what the goals section shows when no
  goals exist (empty state guidance)? [Coverage, Spec §FR-021]
- [ ] CHK047 Are requirements defined for what happens when a student tries to
  set a goal with a past deadline — does the error message explain in
  plain language? [Coverage, Edge Case]
- [ ] CHK048 Are requirements defined for completed goals — does the UI
  distinguish between active and completed goals visually?
  [Coverage, Gap]

## Cross-Cutting UX Requirements

- [ ] CHK049 Are requirements consistent between the three areas regarding
  currency display — is the same currency symbol/format used on dashboard,
  expense form, and savings goals? [Consistency, Spec §FR-033]
- [ ] CHK050 Are requirements consistent regarding empty states across all
  three areas — do they all follow the same pattern of helpful guidance?
  [Consistency, Spec §FR-021]
- [ ] CHK051 Are requirements consistent regarding mobile responsiveness
  across all three areas — do they all define the same breakpoint behavior?
  [Consistency, Spec §FR-022]
- [ ] CHK052 Are requirements consistent regarding immediate update behavior
  across all three areas — do dashboard, expenses, and goals all update
  instantly without page refresh? [Consistency, Spec §FR-010, §FR-027, §FR-029]
- [ ] CHK053 Are accessibility requirements defined for all interactive elements
  across all three areas (keyboard navigation, screen reader labels, focus
  management)? [Coverage, Spec §FR-034, §FR-035]

## Notes

- Checklist validates requirements quality for 3 areas: dashboard, expense
  registration, savings goals — all through the lens of non-financial users.
- Constitution Principles 1 (Enfoque en el Usuario Estudiante) and 3
  (Claridad de Datos) are the primary quality gates.
- Key gaps identified: plain-language explanations for financial terms,
  category icons/descriptions, goal progress explanations, loading states.
- Items marked [Gap] indicate missing requirements that should be added
  before implementation.
