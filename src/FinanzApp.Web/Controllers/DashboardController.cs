using FinanzApp.Web.Models;
using FinanzApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanzApp.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IDashboardService dashboardService,
        UserManager<ApplicationUser> userManager)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? year, int? month)
    {
        var userId = _userManager.GetUserId(User)!;
        var model = await _dashboardService.GetDashboardSummaryAsync(userId, year, month);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetBudget(decimal amount)
    {
        if (amount < 0 || amount > 99_999_999)
        {
            TempData["BudgetError"] = "El presupuesto no puede ser negativo.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var now = DateTime.UtcNow;
        user.MonthlyBudget = amount;
        user.BudgetMonth = now.Month;
        user.BudgetYear = now.Year;

        await _userManager.UpdateAsync(user);

        TempData["BudgetSaved"] = true;
        return RedirectToAction(nameof(Index));
    }
}
