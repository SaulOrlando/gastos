using FinanzApp.Web.Models;

namespace FinanzApp.Web.Repositories;

public interface IExpenseRepository
{
    Task<List<Expense>> GetMonthlyExpensesAsync(string userId, int year, int month);

    Task<Dictionary<ExpenseCategory, decimal>> GetTotalExpensesByCategoryAsync(string userId, int year, int month);

    Task<List<Expense>> GetExpensesSinceAsync(string userId, DateTime fromDate);
}
