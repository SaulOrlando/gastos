namespace FinanzApp.Web.ViewModels;

public class DashboardViewModel
{
    public decimal TotalSpent { get; set; }

    public decimal TotalIncome { get; set; }

    public decimal TotalIncomeCumulative { get; set; }

    public decimal? MonthlyBudget { get; set; }

    public decimal RemainingBudget { get; set; }

    public bool HasExpenses => TotalSpent > 0;

    public bool HasBudget => MonthlyBudget is > 0;

    public string MonthName { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Month { get; set; }

    public bool IsCurrentMonth { get; set; }

    public string CurrencySymbol { get; set; } = string.Empty;

    public string LabelsCategoriaJson { get; set; } = "[]";

    public string ValoresCategoriaJson { get; set; } = "[]";

    public string ColoresCategoriaJson { get; set; } = "[]";

    public string LabelsCategoriaIngresosJson { get; set; } = "[]";

    public string ValoresCategoriaIngresosJson { get; set; } = "[]";

    public string ColoresCategoriaIngresosJson { get; set; } = "[]";

    public string LabelsDiasJson { get; set; } = "[]";

    public string ValoresDiasJson { get; set; } = "[]";

    public string ValoresIngresosDiasJson { get; set; } = "[]";
}
