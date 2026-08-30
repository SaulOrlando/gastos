using FinanzApp.Web.Models;

namespace FinanzApp.Web.Repositories;

public interface IIncomeRepository
{
    Task<List<Income>> GetAllByUserIdAsync(string userId);

    Task<Income?> GetByIdAsync(int id, string userId);

    Task AddAsync(Income income);

    Task UpdateAsync(Income income);

    Task<bool> DeleteAsync(int id, string userId);

    Task<List<Income>> GetMonthlyIncomesAsync(string userId, int year, int month);

    Task<Dictionary<string, decimal>> GetTotalIncomesByCategoryAsync(string userId, int year, int month);

    Task<List<Income>> GetIncomesSinceAsync(string userId, DateTime fromDate);

    Task<decimal> GetTotalIncomesUntilAsync(string userId, DateTime untilDate);

    Task SaveRecurringProgressAsync(ApplicationUser user);
}
