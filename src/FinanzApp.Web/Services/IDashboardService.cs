using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardSummaryAsync(string userId);
}
