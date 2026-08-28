using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface IExpenseService
{
    Task<List<ExpenseListItemViewModel>> GetExpensesForMonthAsync(string userId, int year, int month, int? limit = null);

    Task<ExpenseFormViewModel?> GetExpenseForEditAsync(int id, string userId);

    Task CreateExpenseAsync(ExpenseFormViewModel model, string userId);

    Task<bool> UpdateExpenseAsync(ExpenseFormViewModel model, string userId);

    Task<bool> DeleteExpenseAsync(int id, string userId);

    Task<bool> IsValidCategoryAsync(string category, string userId);
}
