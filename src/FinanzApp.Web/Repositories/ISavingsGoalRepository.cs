using FinanzApp.Web.Models;

namespace FinanzApp.Web.Repositories;

public interface ISavingsGoalRepository
{
    Task<List<SavingsGoal>> GetAllByUserIdAsync(string userId);

    Task<SavingsGoal?> GetByIdAsync(int id, string userId);

    Task AddAsync(SavingsGoal goal);

    Task AddEntryAsync(SavingsEntry entry);

    Task UpdateAsync(SavingsGoal goal);

    Task<bool> DeleteAsync(int id, string userId);
}
