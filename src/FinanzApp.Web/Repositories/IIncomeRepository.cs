using FinanzApp.Web.Models;

namespace FinanzApp.Web.Repositories;

public interface IIncomeRepository
{
    Task<List<Income>> GetAllByUserIdAsync(string userId);

    Task<Income?> GetByIdAsync(int id, string userId);

    Task AddAsync(Income income);

    Task<bool> DeleteAsync(int id, string userId);
}
