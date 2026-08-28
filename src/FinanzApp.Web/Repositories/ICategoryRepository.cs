using FinanzApp.Web.Models;

namespace FinanzApp.Web.Repositories;

public interface ICategoryRepository
{
    Task<List<ExpenseCategory>> GetAllForUserAsync(string userId);

    Task<ExpenseCategory?> GetOwnedByIdAsync(int id, string userId);

    Task AddAsync(ExpenseCategory category);

    Task UpdateAsync(ExpenseCategory category);

    Task<bool> DeleteAsync(int id, string userId);

    Task<bool> NameExistsAsync(string name, string userId, int? excludeId = null);
}