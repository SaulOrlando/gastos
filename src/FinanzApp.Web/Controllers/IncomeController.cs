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
    private readonly ICategoryService _categoryService;
    private readonly UserManager<ApplicationUser> _userManager;

    public IncomeController(
        IIncomeService incomeService,
        ICategoryService categoryService,
        UserManager<ApplicationUser> userManager)
    {
        _incomeService = incomeService;
        _categoryService = categoryService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        return View(await BuildCreateModelAsync(user, new IncomeFormViewModel
        {
            Date = DateTime.UtcNow.Date
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IncomeFormViewModel model)
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

        if (!await _incomeService.IsValidCategoryAsync(model.Category, user.Id))
        {
            ModelState.AddModelError(nameof(model.Category), "Elige una categoría que exista.");
            return View(await BuildCreateModelAsync(user, model));
        }

        await _incomeService.CreateIncomeAsync(model, user.Id);
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = await _incomeService.GetIncomeForEditAsync(id, user.Id);

        if (model is null)
        {
            return NotFound();
        }

        model.Categories = await _categoryService.GetForUserAsync(user.Id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, IncomeFormViewModel model)
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

        if (!await _incomeService.IsValidCategoryAsync(model.Category, user.Id))
        {
            ModelState.AddModelError(nameof(model.Category), "Elige una categoría que exista.");
            model.Categories = await _categoryService.GetForUserAsync(user.Id);
            return View(model);
        }

        model.Id = id;
        var updated = await _incomeService.UpdateIncomeAsync(model, user.Id);

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = await _incomeService.GetIncomeForEditAsync(id, user.Id);

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

        var deleted = await _incomeService.DeleteIncomeAsync(id, user.Id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(string name)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return Json(new { success = false, message = "Debes iniciar sesión." });
        }

        var result = await _categoryService.CreateAsync(name, user.Id);

        if (!result.Success)
        {
            return Json(new { success = false, message = result.Error });
        }

        return Json(new
        {
            success = true,
            id = result.Category!.Id,
            name = result.Category.Name
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(int id, string name)
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            return Json(new { success = false, message = "Debes iniciar sesión." });
        }

        var result = await _categoryService.UpdateAsync(id, name, user.Id);

        if (!result.Success)
        {
            return Json(new { success = false, message = result.Error });
        }

        return Json(new
        {
            success = true,
            id = result.Category!.Id,
            name = result.Category.Name
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

    private async Task<IncomeFormViewModel> BuildCreateModelAsync(ApplicationUser user, IncomeFormViewModel model)
    {
        var now = DateTime.UtcNow;

        model.Categories = await _categoryService.GetForUserAsync(user.Id);
        model.CurrencySymbol = GetCurrencySymbol(user.Currency);
        model.CurrentMonthName = now.ToString("MMMM yyyy");
        model.CurrentMonthIncomes = await _incomeService.GetIncomesForMonthAsync(user.Id, now.Year, now.Month, 5);

        return model;
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