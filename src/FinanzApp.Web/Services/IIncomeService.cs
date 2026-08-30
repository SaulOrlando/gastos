using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface IIncomeService
{
    Task CreateIncomeAsync(IncomeFormViewModel model, string userId);

    Task<IncomeFormViewModel?> GetIncomeForEditAsync(int id, string userId);

    Task<bool> UpdateIncomeAsync(IncomeFormViewModel model, string userId);

    Task<bool> DeleteIncomeAsync(int id, string userId);

    Task<bool> IsValidCategoryAsync(string category, string userId);

    Task<List<IncomeListItemViewModel>> GetIncomesForMonthAsync(string userId, int year, int month, int? limit = null);

    Task<List<IncomeListItemViewModel>> GetAllIncomesAsync(string userId);
}