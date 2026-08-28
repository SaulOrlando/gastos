using FinanzApp.Web.Models;

namespace FinanzApp.Web.Repositories;

public interface IExpenseRepository
{
    Task<List<Expense>> GetMonthlyExpensesAsync(string userId, int year, int month);

    Task<Dictionary<string, decimal>> GetTotalExpensesByCategoryAsync(string userId, int year, int month);

    Task<List<Expense>> GetExpensesSinceAsync(string userId, DateTime fromDate);

    Task<decimal> GetTotalExpensesUntilAsync(string userId, DateTime untilDate);

    Task<Expense?> GetByIdAsync(int id, string userId);

    Task AddAsync(Expense expense);

    Task UpdateAsync(Expense expense);

    Task<bool> DeleteAsync(int id, string userId);

    Task<List<(int Year, int Month)>> GetDistinctMonthsAsync(string userId);
}
