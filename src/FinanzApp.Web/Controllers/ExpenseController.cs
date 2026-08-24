using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinanzApp.Web.Models;
using FinanzApp.Web.Services;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Controllers;

[Authorize]
public class ExpenseController : Controller
{
    private readonly IExpenseService _expenseService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExpenseController(
        IExpenseService expenseService,
        UserManager<ApplicationUser> userManager)
    {
        _expenseService = expenseService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var items = await _expenseService.GetExpensesForUserAsync(user.Id);

        return View(new ExpenseListViewModel
        {
            Items = items,
            CurrencySymbol = GetCurrencySymbol(user.Currency)
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ExpenseFormViewModel { Date = DateTime.UtcNow.Date });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseFormViewModel model)
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

        await _expenseService.CreateExpenseAsync(model, user.Id);
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

        var model = await _expenseService.GetExpenseForEditAsync(id, user.Id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExpenseFormViewModel model)
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

        model.Id = id;
        var updated = await _expenseService.UpdateExpenseAsync(model, user.Id);

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = await _expenseService.GetExpenseForEditAsync(id, user.Id);

        if (model is null)
        {
            return NotFound();
        }

        ViewBag.CurrencySymbol = GetCurrencySymbol(user.Currency);
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var deleted = await _expenseService.DeleteExpenseAsync(id, user.Id);

        if (!deleted)
        {
            return NotFound();
        }

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
