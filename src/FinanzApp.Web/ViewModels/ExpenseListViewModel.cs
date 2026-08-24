namespace FinanzApp.Web.ViewModels;

public class ExpenseListViewModel
{
    public List<ExpenseListItemViewModel> Items { get; set; } = new();

    public string CurrencySymbol { get; set; } = string.Empty;

    public decimal TotalAmount => Items.Sum(i => i.Amount);
}
