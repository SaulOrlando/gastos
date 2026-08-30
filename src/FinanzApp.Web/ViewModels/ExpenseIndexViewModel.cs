namespace FinanzApp.Web.ViewModels;

public class ExpenseIndexViewModel
{
    public decimal TotalExpenses { get; set; }

    public string MonthName { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Month { get; set; }

    public bool IsCurrentMonth { get; set; }

    public string CurrencySymbol { get; set; } = string.Empty;

    public List<CategoryViewModel> Categories { get; set; } = new();

    public List<ExpenseListItemViewModel> Expenses { get; set; } = new();

    public List<ExpenseMonthNavItem> AvailableMonths { get; set; } = new();
}

public class ExpenseMonthNavItem
{
    public int Year { get; set; }

    public int Month { get; set; }

    public string MonthName { get; set; } = string.Empty;

    public decimal TotalExpenses { get; set; }

    public bool IsSelected { get; set; }
}
