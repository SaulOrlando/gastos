namespace FinanzApp.Web.ViewModels;

public class HistoryViewModel
{
    public List<MonthHistoryItem> Months { get; set; } = new();

    public string CurrencySymbol { get; set; } = string.Empty;
}

public class MonthHistoryItem
{
    public int Year { get; set; }

    public int Month { get; set; }

    public string MonthName { get; set; } = string.Empty;

    public decimal TotalExpenses { get; set; }

    public decimal TotalIncome { get; set; }

    public bool IsCurrentMonth { get; set; }

    public decimal Remaining => TotalIncome - TotalExpenses;
}

public class MonthlySummaryViewModel
{
    public int Year { get; set; }

    public int Month { get; set; }

    public string MonthName { get; set; } = string.Empty;

    public decimal TotalExpenses { get; set; }

    public decimal TotalIncome { get; set; }

    public decimal Remaining => TotalIncome - TotalExpenses;

    public bool IsCurrentMonth { get; set; }

    public string CurrencySymbol { get; set; } = string.Empty;

    public List<MonthCategorySummary> ExpenseCategories { get; set; } = new();

    public List<MonthIncomeSummary> IncomeCategories { get; set; } = new();
}

public class MonthCategorySummary
{
    public string CategoryName { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public int Count { get; set; }

    public decimal Percent { get; set; }
}

public class MonthIncomeSummary
{
    public string CategoryName { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public int Count { get; set; }

    public decimal Percent { get; set; }
}
