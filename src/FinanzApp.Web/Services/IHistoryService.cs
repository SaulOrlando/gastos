using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface IHistoryService
{
    Task<HistoryViewModel> GetMonthlyHistoryAsync(string userId);

    Task<MonthlySummaryViewModel?> GetMonthlySummaryAsync(string userId, int year, int month);
}
