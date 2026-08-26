using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface IIncomeService
{
    Task CreateIncomeAsync(IncomeFormViewModel model, string userId);
}
