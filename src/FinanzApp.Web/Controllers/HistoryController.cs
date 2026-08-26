using FinanzApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanzApp.Web.Controllers;

[Authorize]
public class HistoryController : Controller
{
    private readonly IHistoryService _historyService;
    private readonly UserManager<Models.ApplicationUser> _userManager;

    public HistoryController(
        IHistoryService historyService,
        UserManager<Models.ApplicationUser> userManager)
    {
        _historyService = historyService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = await _historyService.GetMonthlyHistoryAsync(user.Id);
        return View(model);
    }
}
