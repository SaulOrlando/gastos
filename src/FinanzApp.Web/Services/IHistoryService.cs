using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface IHistoryService
{
    Task<HistoryViewModel> GetMonthlyHistoryAsync(string userId);
}
