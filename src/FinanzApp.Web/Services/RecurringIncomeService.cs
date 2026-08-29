using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;

namespace FinanzApp.Web.Services;

public class RecurringIncomeService : IRecurringIncomeService
{
    private const string RecurringCategory = "Sueldo";

    private readonly IIncomeRepository _incomeRepository;

    public RecurringIncomeService(IIncomeRepository incomeRepository)
    {
        _incomeRepository = incomeRepository;
    }

    public async Task ProcessAsync(ApplicationUser user)
    {
        var amount = user.SalaryAmount ?? 0m;
        if (amount <= 0m)
        {
            return;
        }

        var today = DateTime.UtcNow.Date;

        if (!TryGetSchedule(user, today, out var schedule))
        {
            return;
        }

        var lastProcessed = user.LastRecurringIncomeAt?.Date ?? schedule.StartDate.AddDays(-1);
        if (lastProcessed >= today)
        {
            return;
        }

        var earliestDue = lastProcessed.AddDays(1);
        var dueDates = BuildDueDates(schedule, earliestDue, today);

        if (dueDates.Count == 0)
        {
            return;
        }

        foreach (var due in dueDates)
        {
            var existing = await GetRecurringIncomeOnAsync(user.Id, due);
            if (existing) continue;

            var income = new Income
            {
                UserId = user.Id,
                Amount = amount,
                Category = RecurringCategory,
                Date = due,
                Note = "Depósito automático",
                IsRecurring = true,
                CreatedAt = DateTime.UtcNow
            };

            await _incomeRepository.AddAsync(income);
        }

        user.LastRecurringIncomeAt = today;
        await _incomeRepository.SaveRecurringProgressAsync(user);
    }

    private static bool TryGetSchedule(ApplicationUser user, DateTime today, out RecurringSchedule schedule)
    {
        schedule = default;

        var startDate = user.DepositStartDate?.Date;

        switch (user.DepositFrequency)
        {
            case "Mensual":
                var mensualDay = startDate?.Day ?? 1;
                var mensualStart = AnchorMonthly(startDate, today, mensualDay);
                schedule = new RecurringSchedule(mensualStart, Recurrence.Monthly, mensualDay);
                return true;

            case "Quincenal":
                var quincenalStart = startDate ?? today;
                schedule = new RecurringSchedule(quincenalStart, Recurrence.Days, 15);
                return true;

            case "Personalizado":
                if (!startDate.HasValue || user.DepositIntervalDays is not > 0)
                {
                    return false;
                }
                schedule = new RecurringSchedule(startDate.Value, Recurrence.Days, user.DepositIntervalDays.Value);
                return true;

            default:
                return false;
        }
    }

    private static DateTime AnchorMonthly(DateTime? startDate, DateTime today, int dayOfMonth)
    {
        if (startDate.HasValue)
        {
            return new DateTime(startDate.Value.Year, startDate.Value.Month, ClampDay(startDate.Value.Year, startDate.Value.Month, dayOfMonth));
        }

        // Sin fecha definida: anclar al primer día del mes en curso.
        return new DateTime(today.Year, today.Month, 1);
    }

    private static int ClampDay(int year, int month, int day)
    {
        var lastDay = DateTime.DaysInMonth(year, month);
        return Math.Min(day, lastDay);
    }

    private static List<DateTime> BuildDueDates(RecurringSchedule schedule, DateTime fromInclusive, DateTime untilInclusive)
    {
        var dates = new List<DateTime>();

        switch (schedule.Kind)
        {
            case Recurrence.Monthly:
                var cursor = schedule.StartDate;
                while (cursor <= untilInclusive)
                {
                    if (cursor >= fromInclusive)
                    {
                        dates.Add(cursor);
                    }
                    cursor = NextMonthly(cursor, schedule.DayOfMonth);
                }
                break;

            case Recurrence.Days:
                var interval = schedule.IntervalDays;
                var offset = ((fromInclusive - schedule.StartDate).Days + interval - 1) / interval * interval;
                var dayCursor = schedule.StartDate.AddDays(offset);
                while (dayCursor <= untilInclusive)
                {
                    if (dayCursor >= fromInclusive)
                    {
                        dates.Add(dayCursor);
                    }
                    dayCursor = dayCursor.AddDays(interval);
                }
                break;
        }

        return dates;
    }

    private static DateTime NextMonthly(DateTime current, int dayOfMonth)
    {
        var next = current.AddMonths(1);
        return new DateTime(next.Year, next.Month, ClampDay(next.Year, next.Month, dayOfMonth));
    }

    private async Task<bool> GetRecurringIncomeOnAsync(string userId, DateTime date)
    {
        var incomes = await _incomeRepository.GetIncomesSinceAsync(userId, date.Date);
        return incomes.Any(i => i.IsRecurring && i.Date.Date == date.Date);
    }

    private enum Recurrence
    {
        Days,
        Monthly
    }

    private readonly struct RecurringSchedule
    {
        public RecurringSchedule(DateTime startDate, Recurrence kind, int intervalDays)
        {
            StartDate = startDate;
            Kind = kind;
            IntervalDays = intervalDays;
            DayOfMonth = startDate.Day;
        }

        public DateTime StartDate { get; }
        public Recurrence Kind { get; }
        public int IntervalDays { get; }
        public int DayOfMonth { get; }
    }
}
