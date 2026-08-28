namespace FinanzApp.Web.ViewModels;

public class IncomeListItemViewModel
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Category { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string? Note { get; set; }
}