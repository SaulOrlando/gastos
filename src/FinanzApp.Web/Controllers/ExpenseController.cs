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
    private readonly ICategoryService _categoryService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExpenseController(
        IExpenseService expenseService,
        ICategoryService categoryService,
        UserManager<ApplicationUser> userManager)
    {
        _expenseService = expenseService;
        _categoryService = categoryService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? year, int? month)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var now = DateTime.UtcNow;
        var targetYear = year ?? now.Year;
        var targetMonth = month ?? now.Month;

        var allExpenses = await _expenseService.GetAllExpensesAsync(user.Id);

        var availableMonths = allExpenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .OrderByDescending(g => g.Key.Year)
            .ThenByDescending(g => g.Key.Month)
            .Select(g => new ExpenseMonthNavItem
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                TotalExpenses = g.Sum(e => e.Amount),
                IsSelected = g.Key.Year == targetYear && g.Key.Month == targetMonth
            })
            .ToList();

        if (!availableMonths.Any(m => m.IsSelected))
        {
            availableMonths.Insert(0, new ExpenseMonthNavItem
            {
                Year = targetYear,
                Month = targetMonth,
                MonthName = new DateTime(targetYear, targetMonth, 1).ToString("MMMM yyyy"),
                TotalExpenses = 0,
                IsSelected = true
            });
        }

        var monthExpenses = allExpenses
            .Where(e => e.Date.Year == targetYear && e.Date.Month == targetMonth)
            .OrderByDescending(e => e.Date)
            .ToList();

        var model = new ExpenseIndexViewModel
        {
            TotalExpenses = monthExpenses.Sum(e => e.Amount),
            MonthName = new DateTime(targetYear, targetMonth, 1).ToString("MMMM yyyy"),
            Year = targetYear,
            Month = targetMonth,
            IsCurrentMonth = targetYear == now.Year && targetMonth == now.Month,
            CurrencySymbol = GetCurrencySymbol(user.Currency),
            Categories = await _categoryService.GetForUserAsync(user.Id),
            Expenses = monthExpenses,
            AvailableMonths = availableMonths
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? year, int? month)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        return View(await BuildCreateModelAsync(user, new ExpenseFormViewModel
        {
            Date = DefaultDate(year, month)
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseFormViewModel model)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!ModelState.IsValid)
        {
            return View(await BuildCreateModelAsync(user, model));
        }

        if (!await _expenseService.IsValidCategoryAsync(model.Category, user.Id))
        {
            ModelState.AddModelError(nameof(model.Category), "Elige una categoría que exista.");
            return View(await BuildCreateModelAsync(user, model));
        }

        await _expenseService.CreateExpenseAsync(model, user.Id);
        return RedirectToAction("Index", "Dashboard", new { year = model.Date.Year, month = model.Date.Month });
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

        model.Categories = await _categoryService.GetForUserAsync(user.Id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExpenseFormViewModel model)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await _categoryService.GetForUserAsync(user.Id);
            return View(model);
        }

        if (!await _expenseService.IsValidCategoryAsync(model.Category, user.Id))
        {
            ModelState.AddModelError(nameof(model.Category), "Elige una categoría que exista.");
            model.Categories = await _categoryService.GetForUserAsync(user.Id);
            return View(model);
        }

        model.Id = id;
        var updated = await _expenseService.UpdateExpenseAsync(model, user.Id);

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction("Index", "Dashboard", new { year = model.Date.Year, month = model.Date.Month });
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

        var existing = await _expenseService.GetExpenseForEditAsync(id, user.Id);

        var deleted = await _expenseService.DeleteExpenseAsync(id, user.Id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction("Index", "Expense", new { year = existing?.Date.Year, month = existing?.Date.Month });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(string name, string color)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return Json(new { success = false, message = "Debes iniciar sesión." });
        }

        var result = await _categoryService.CreateAsync(name, user.Id, color);

        if (!result.Success)
        {
            return Json(new { success = false, message = result.Error });
        }

        return Json(new
        {
            success = true,
            id = result.Category!.Id,
            name = result.Category.Name,
            color = result.Category.Color
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(int id, string name, string color)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return Json(new { success = false, message = "Debes iniciar sesión." });
        }

        var result = await _categoryService.UpdateAsync(id, name, user.Id, color);

        if (!result.Success)
        {
            return Json(new { success = false, message = result.Error });
        }

        return Json(new
        {
            success = true,
            id = result.Category!.Id,
            name = result.Category.Name,
            color = result.Category.Color
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return Json(new { success = false, message = "Debes iniciar sesión." });
        }

        var result = await _categoryService.DeleteAsync(id, user.Id);

        if (!result.Success)
        {
            return Json(new { success = false, message = result.Error });
        }

        return Json(new { success = true });
    }

    private async Task<ExpenseFormViewModel> BuildCreateModelAsync(ApplicationUser user, ExpenseFormViewModel model)
    {
        model.Categories = await _categoryService.GetForUserAsync(user.Id);
        model.CurrencySymbol = GetCurrencySymbol(user.Currency);

        return model;
    }

    private Task<ApplicationUser?> GetCurrentUserAsync() => _userManager.GetUserAsync(User);

    private static DateTime DefaultDate(int? year, int? month)
    {
        var now = DateTime.UtcNow.Date;

        if (!year.HasValue || !month.HasValue)
        {
            return now;
        }

        if (year.Value == now.Year && month.Value == now.Month)
        {
            return now;
        }

        var firstDay = new DateTime(year.Value, month.Value, 1);
        return firstDay;
    }

    private static string GetCurrencySymbol(string currency) => currency switch
    {
        "USD" => "US$",
        "EUR" => "€",
        "MXN" => "$",
        "COP" => "COL$",
        _ => $"{currency} "
    };
}
