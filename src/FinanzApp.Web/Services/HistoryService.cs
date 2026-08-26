using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace FinanzApp.Web.Services;

public class HistoryService : IHistoryService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    private static readonly Dictionary<ExpenseCategory, string> CategoryNames = new()
    {
        [ExpenseCategory.Mensualidad] = "Mensualidad",
        [ExpenseCategory.Transporte] = "Transporte",
        [ExpenseCategory.Comida] = "Comida",
        [ExpenseCategory.Entretenimiento] = "Entretenimiento"
    };

    public HistoryService(
        IExpenseRepository expenseRepository,
        UserManager<ApplicationUser> userManager)
    {
        _expenseRepository = expenseRepository;
        _userManager = userManager;
    }

    public async Task<HistoryViewModel> GetMonthlyHistoryAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"No existe el usuario {userId}.");

        var months = await _expenseRepository.GetDistinctMonthsAsync(userId);
        var monthItems = new List<MonthHistoryItem>();

        foreach (var (year, month) in months)
        {
            var expenses = await _expenseRepository.GetMonthlyExpensesAsync(userId, year, month);
            var totalsByCategory = await _expenseRepository.GetTotalExpensesByCategoryAsync(userId, year, month);

            var totalAmount = expenses.Sum(e => e.Amount);
            var now = DateTime.UtcNow;
            var isCurrentMonth = year == now.Year && month == now.Month;

            var categorySummaries = totalsByCategory.Select(kvp => new MonthCategorySummary
            {
                CategoryName = CategoryNames.GetValueOrDefault(kvp.Key, kvp.Key.ToString()),
                Total = Math.Round(kvp.Value, 2),
                Count = expenses.Count(e => e.Category == kvp.Key),
                Percent = totalAmount > 0 ? Math.Round(kvp.Value / totalAmount * 100, 1) : 0
            }).OrderByDescending(c => c.Total).ToList();

            monthItems.Add(new MonthHistoryItem
            {
                Year = year,
                Month = month,
                MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy"),
                TotalAmount = Math.Round(totalAmount, 2),
                ExpenseCount = expenses.Count,
                Budget = isCurrentMonth ? user.MonthlyBudget ?? 0 : 0,
                Categories = categorySummaries
            });
        }

        return new HistoryViewModel
        {
            Months = monthItems,
            CurrencySymbol = GetCurrencySymbol(user.Currency)
        };
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
