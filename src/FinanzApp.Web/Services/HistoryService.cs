using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace FinanzApp.Web.Services;

public class HistoryService : IHistoryService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IIncomeRepository _incomeRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    private static readonly Dictionary<ExpenseCategory, string> ExpenseCategoryNames = new()
    {
        [ExpenseCategory.Mensualidad] = "Mensualidad",
        [ExpenseCategory.Transporte] = "Transporte",
        [ExpenseCategory.Comida] = "Comida",
        [ExpenseCategory.Entretenimiento] = "Entretenimiento"
    };

    private static readonly Dictionary<IncomeCategory, string> IncomeCategoryNames = new()
    {
        [IncomeCategory.Beca] = "Beca",
        [IncomeCategory.Mesada] = "Mesada",
        [IncomeCategory.Salario] = "Salario",
        [IncomeCategory.Otro] = "Otro"
    };

    public HistoryService(
        IExpenseRepository expenseRepository,
        IIncomeRepository incomeRepository,
        UserManager<ApplicationUser> userManager)
    {
        _expenseRepository = expenseRepository;
        _incomeRepository = incomeRepository;
        _userManager = userManager;
    }

    public async Task<HistoryViewModel> GetMonthlyHistoryAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"No existe el usuario {userId}.");

        var expenseMonths = await _expenseRepository.GetDistinctMonthsAsync(userId);
        var incomeMonths = await GetDistinctIncomeMonthsAsync(userId);

        var allMonths = expenseMonths
            .Union(incomeMonths)
            .Distinct()
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ToList();

        var monthItems = new List<MonthHistoryItem>();
        var now = DateTime.UtcNow;

        foreach (var (year, month) in allMonths)
        {
            var expenses = await _expenseRepository.GetMonthlyExpensesAsync(userId, year, month);
            var incomes = await _incomeRepository.GetMonthlyIncomesAsync(userId, year, month);

            monthItems.Add(new MonthHistoryItem
            {
                Year = year,
                Month = month,
                MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy"),
                TotalExpenses = Math.Round(expenses.Sum(e => e.Amount), 2),
                TotalIncome = Math.Round(incomes.Sum(i => i.Amount), 2),
                IsCurrentMonth = year == now.Year && month == now.Month
            });
        }

        return new HistoryViewModel
        {
            Months = monthItems,
            CurrencySymbol = GetCurrencySymbol(user.Currency)
        };
    }

    public async Task<MonthlySummaryViewModel?> GetMonthlySummaryAsync(string userId, int year, int month)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"No existe el usuario {userId}.");

        var expenses = await _expenseRepository.GetMonthlyExpensesAsync(userId, year, month);
        var incomes = await _incomeRepository.GetMonthlyIncomesAsync(userId, year, month);

        if (!expenses.Any() && !incomes.Any())
        {
            return null;
        }

        var totalExpenses = expenses.Sum(e => e.Amount);
        var totalIncome = incomes.Sum(i => i.Amount);
        var now = DateTime.UtcNow;

        var expenseCategories = expenses
            .GroupBy(e => e.Category)
            .Select(g => new MonthCategorySummary
            {
                CategoryName = ExpenseCategoryNames.GetValueOrDefault(g.Key, g.Key.ToString()),
                Total = Math.Round(g.Sum(e => e.Amount), 2),
                Count = g.Count(),
                Percent = totalExpenses > 0 ? Math.Round(g.Sum(e => e.Amount) / totalExpenses * 100, 1) : 0
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        var incomeCategories = incomes
            .GroupBy(i => i.Category)
            .Select(g => new MonthIncomeSummary
            {
                CategoryName = IncomeCategoryNames.GetValueOrDefault(g.Key, g.Key.ToString()),
                Total = Math.Round(g.Sum(i => i.Amount), 2),
                Count = g.Count(),
                Percent = totalIncome > 0 ? Math.Round(g.Sum(i => i.Amount) / totalIncome * 100, 1) : 0
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        return new MonthlySummaryViewModel
        {
            Year = year,
            Month = month,
            MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy"),
            TotalExpenses = Math.Round(totalExpenses, 2),
            TotalIncome = Math.Round(totalIncome, 2),
            IsCurrentMonth = year == now.Year && month == now.Month,
            CurrencySymbol = GetCurrencySymbol(user.Currency),
            ExpenseCategories = expenseCategories,
            IncomeCategories = incomeCategories
        };
    }

    private async Task<List<(int Year, int Month)>> GetDistinctIncomeMonthsAsync(string userId)
    {
        var incomes = await _incomeRepository.GetAllByUserIdAsync(userId);
        return incomes
            .Select(i => (i.Date.Year, i.Date.Month))
            .Distinct()
            .ToList();
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
