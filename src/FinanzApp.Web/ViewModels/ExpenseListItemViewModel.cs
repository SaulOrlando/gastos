using FinanzApp.Web.Models;

namespace FinanzApp.Web.ViewModels;

public class ExpenseListItemViewModel
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public ExpenseCategory Category { get; set; }

    public DateTime Date { get; set; }

    public string? Note { get; set; }
}
