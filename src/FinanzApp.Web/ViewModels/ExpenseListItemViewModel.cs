namespace FinanzApp.Web.ViewModels;

public class ExpenseListItemViewModel
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Category { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string? Note { get; set; }

    public string Color { get; set; } = "#21C3D6";
}
