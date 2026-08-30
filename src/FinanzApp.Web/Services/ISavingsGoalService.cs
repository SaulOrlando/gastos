using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface ISavingsGoalService
{
    Task<List<SavingsGoalItemViewModel>> GetActiveGoalsAsync(string userId, string currencySymbol);

    Task<bool> CreateGoalAsync(CreateGoalViewModel model, string userId);

    Task<bool> AddFundsAsync(AddFundsViewModel model, string userId);

    Task<CreateGoalViewModel?> GetGoalForEditAsync(int id, string userId, string currencySymbol);

    Task<bool> UpdateGoalAsync(CreateGoalViewModel model, string userId);
}
