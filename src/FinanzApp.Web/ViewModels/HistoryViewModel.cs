namespace FinanzApp.Web.ViewModels;

public class HistoryViewModel
{
    public List<MonthHistoryItem> Months { get; set; } = new();

    public string CurrencySymbol { get; set; } = string.Empty;

    public int TotalExpenses => Months.Sum(m => m.ExpenseCount);

    public decimal TotalAllTime => Months.Sum(m => m.TotalAmount);
}

public class MonthHistoryItem
{
    public int Year { get; set; }

    public int Month { get; set; }

    public string MonthName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public int ExpenseCount { get; set; }

    public decimal Budget { get; set; }

    public decimal Remaining => Budget > 0 ? Budget - TotalAmount : 0;

    public bool HasBudget => Budget > 0;

    public bool IsOverBudget => HasBudget && TotalAmount > Budget;

    public decimal BudgetPercent => HasBudget
        ? Math.Min(100, Math.Max(0, (int)(TotalAmount / Budget * 100)))
        : 0;

    public List<MonthCategorySummary> Categories { get; set; } = new();
}

public class MonthCategorySummary
{
    public string CategoryName { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public int Count { get; set; }

    public decimal Percent { get; set; }
}
