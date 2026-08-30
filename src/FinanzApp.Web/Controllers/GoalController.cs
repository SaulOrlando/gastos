using FinanzApp.Web.Models;
using FinanzApp.Web.Services;
using FinanzApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanzApp.Web.Controllers;

[Authorize]
public class GoalController : Controller
{
    private readonly ISavingsGoalService _savingsGoalService;
    private readonly UserManager<ApplicationUser> _userManager;

    private static readonly string[] SuggestedCategories =
    {
        "Viaje", "Emergencia", "Tecnología", "Educación", "Ropa", "Otro"
    };

    public GoalController(
        ISavingsGoalService savingsGoalService,
        UserManager<ApplicationUser> userManager)
    {
        _savingsGoalService = savingsGoalService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = new SavingsGoalListViewModel
        {
            CurrencySymbol = GetCurrencySymbol(user.Currency),
            Goals = await _savingsGoalService.GetActiveGoalsAsync(user.Id, GetCurrencySymbol(user.Currency))
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.Categories = SuggestedCategories;
        return View(new CreateGoalViewModel
        {
            CurrencySymbol = GetCurrencySymbol(user.Currency),
            SalaryAmount = user.SalaryAmount ?? 0m,
            Deadline = DateTime.UtcNow.Date.AddMonths(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGoalViewModel model)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.Categories = SuggestedCategories;
        model.CurrencySymbol = GetCurrencySymbol(user.Currency);
        model.SalaryAmount = user.SalaryAmount ?? 0m;

        ValidateMonthlyContribution(model, model.SalaryAmount);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _savingsGoalService.CreateGoalAsync(model, user.Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.Categories = SuggestedCategories;
        var model = await _savingsGoalService.GetGoalForEditAsync(id, user.Id, GetCurrencySymbol(user.Currency));

        if (model is null)
        {
            return NotFound();
        }

        model.SalaryAmount = user.SalaryAmount ?? 0m;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CreateGoalViewModel model)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.Categories = SuggestedCategories;
        model.CurrencySymbol = GetCurrencySymbol(user.Currency);
        model.SalaryAmount = user.SalaryAmount ?? 0m;

        ValidateMonthlyContribution(model, model.SalaryAmount);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _savingsGoalService.UpdateGoalAsync(model, user.Id);

        if (!updated)
        {
            TempData["GoalError"] = "No se pudo actualizar la meta. Inténtalo de nuevo.";
            return RedirectToAction(nameof(Index));
        }

        TempData["GoalSaved"] = true;
        return RedirectToAction(nameof(Index));
    }

    private void ValidateMonthlyContribution(CreateGoalViewModel model, decimal salaryAmount)
    {
        if (model.MonthlyContribution > 0m && salaryAmount < 1m)
        {
            ModelState.AddModelError(nameof(CreateGoalViewModel.MonthlyContribution),
                "Con un sueldo de 0.00 (o menor a 1.00) no se puede descontar de tu sueldo. Configura tu sueldo en Ajustes.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFunds(int goalId, decimal amount)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = new AddFundsViewModel
        {
            GoalId = goalId,
            Amount = amount
        };

        if (!ModelState.IsValid || model.Amount <= 0)
        {
            TempData["GoalError"] = "El monto debe ser mayor a 0.";
            return RedirectToAction(nameof(Index));
        }

        var added = await _savingsGoalService.AddFundsAsync(model, user.Id);

        if (!added)
        {
            TempData["GoalError"] = "No se encontró la meta. Inténtalo de nuevo.";
            return RedirectToAction(nameof(Index));
        }

        TempData["GoalSaved"] = true;
        return RedirectToAction(nameof(Index));
    }

    private Task<ApplicationUser?> GetCurrentUserAsync() => _userManager.GetUserAsync(User);

    private static string GetCurrencySymbol(string currency) => currency switch
    {
        "USD" => "US$",
        "EUR" => "€",
        "MXN" => "$",
        "COP" => "COL$",
        _ => $"{currency} "
    };
}
