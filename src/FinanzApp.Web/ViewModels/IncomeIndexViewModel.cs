namespace FinanzApp.Web.ViewModels;

public class IncomeIndexViewModel
{
    public decimal TotalIncomes { get; set; }

    public string MonthName { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Month { get; set; }

    public bool IsCurrentMonth { get; set; }

    public string CurrencySymbol { get; set; } = string.Empty;

    public List<CategoryViewModel> Categories { get; set; } = new();

    public List<IncomeListItemViewModel> Incomes { get; set; } = new();

    public List<IncomeMonthNavItem> AvailableMonths { get; set; } = new();
}

public class IncomeMonthNavItem
{
    public int Year { get; set; }

    public int Month { get; set; }

    public string MonthName { get; set; } = string.Empty;

    public decimal TotalIncomes { get; set; }

    public bool IsSelected { get; set; }
}