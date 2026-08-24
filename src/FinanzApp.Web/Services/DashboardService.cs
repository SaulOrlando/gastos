using System.Text.Json;
using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace FinanzApp.Web.Services;

public class DashboardService : IDashboardService
{
    private const int TrendDays = 30;

    private static readonly Dictionary<ExpenseCategory, string> CategoryColors = new()
    {
        [ExpenseCategory.Mensualidad] = "#14532d",
        [ExpenseCategory.Transporte] = "#7c3aed",
        [ExpenseCategory.Comida] = "#475569",
        [ExpenseCategory.Entretenimiento] = "#f59e0b"
    };

    private readonly IExpenseRepository _expenseRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(
        IExpenseRepository expenseRepository,
        UserManager<ApplicationUser> userManager)
    {
        _expenseRepository = expenseRepository;
        _userManager = userManager;
    }

    public async Task<DashboardViewModel> GetDashboardSummaryAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"No existe el usuario {userId}.");

        var now = DateTime.UtcNow;

        var monthlyExpenses = await _expenseRepository.GetMonthlyExpensesAsync(userId, now.Year, now.Month);
        var totalsByCategory = await _expenseRepository.GetTotalExpensesByCategoryAsync(userId, now.Year, now.Month);
        var recentExpenses = await _expenseRepository.GetExpensesSinceAsync(userId, now.Date.AddDays(-(TrendDays - 1)));

        var totalSpent = monthlyExpenses.Sum(e => e.Amount);
        var monthlyBudget = user.MonthlyBudget;

        return new DashboardViewModel
        {
            TotalSpent = totalSpent,
            MonthlyBudget = monthlyBudget,
            RemainingBudget = (monthlyBudget ?? 0) - totalSpent,
            MonthName = now.ToString("MMMM yyyy"),
            CurrencySymbol = GetCurrencySymbol(user.Currency),
            LabelsCategoriaJson = JsonSerializer.Serialize(
                totalsByCategory.Keys.Select(c => c.ToString()).ToArray()),
            ValoresCategoriaJson = JsonSerializer.Serialize(
                totalsByCategory.Values.Select(v => Math.Round(v, 2)).ToArray()),
            ColoresCategoriaJson = JsonSerializer.Serialize(
                totalsByCategory.Keys.Select(c => CategoryColors[c]).ToArray()),
            LabelsDiasJson = JsonSerializer.Serialize(BuildDayLabels(now)),
            ValoresDiasJson = JsonSerializer.Serialize(BuildDailyTotals(recentExpenses, now))
        };
    }

    private static decimal[] BuildDailyTotals(List<Expense> expenses, DateTime today)
    {
        var dailyTotals = new decimal[TrendDays];
        var firstDay = today.Date.AddDays(-(TrendDays - 1));

        foreach (var expense in expenses)
        {
            var dayIndex = (expense.Date.Date - firstDay).Days;
            if (dayIndex >= 0 && dayIndex < TrendDays)
            {
                dailyTotals[dayIndex] += expense.Amount;
            }
        }

        return dailyTotals.Select(v => Math.Round(v, 2)).ToArray();
    }

    private static string[] BuildDayLabels(DateTime today)
    {
        var labels = new string[TrendDays];
        for (var i = 0; i < TrendDays; i++)
        {
            labels[i] = today.Date.AddDays(i - (TrendDays - 1)).ToString("dd/MM");
        }

        return labels;
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
