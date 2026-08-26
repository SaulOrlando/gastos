using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinanzApp.Web.Models;
using FinanzApp.Web.Services;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Controllers;

[Authorize]
public class IncomeController : Controller
{
    private readonly IIncomeService _incomeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public IncomeController(
        IIncomeService incomeService,
        UserManager<ApplicationUser> userManager)
    {
        _incomeService = incomeService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new IncomeFormViewModel { Date = DateTime.UtcNow.Date });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IncomeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        await _incomeService.CreateIncomeAsync(model, user.Id);
        return RedirectToAction("Index", "Dashboard");
    }

    private Task<ApplicationUser?> GetCurrentUserAsync() => _userManager.GetUserAsync(User);
}
