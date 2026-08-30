using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryService _categoryService;

    public ExpenseService(
        IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        ICategoryService categoryService)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _categoryService = categoryService;
    }

    public async Task<List<ExpenseListItemViewModel>> GetExpensesForMonthAsync(string userId, int year, int month, int? limit = null)
    {
        var expenses = await _expenseRepository.GetMonthlyExpensesAsync(userId, year, month);

        if (limit.HasValue)
        {
            expenses = expenses.OrderByDescending(e => e.Date).Take(limit.Value).ToList();
        }

        return await MapToListItemAsync(expenses, userId);
    }

    public async Task<List<ExpenseListItemViewModel>> GetAllExpensesAsync(string userId)
    {
        var expenses = await _expenseRepository.GetAllByUserIdAsync(userId);
        return await MapToListItemAsync(expenses, userId);
    }

    private async Task<List<ExpenseListItemViewModel>> MapToListItemAsync(IEnumerable<Expense> expenses, string userId)
    {
        var colors = await _categoryService.GetColorsByNamesAsync(userId);

        return expenses.Select(e => new ExpenseListItemViewModel
        {
            Id = e.Id,
            Amount = e.Amount,
            Category = e.Category,
            Date = e.Date,
            Note = e.Note,
            Color = colors.TryGetValue(e.Category, out var color) ? color : "#21C3D6"
        }).ToList();
    }

    public async Task<ExpenseFormViewModel?> GetExpenseForEditAsync(int id, string userId)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, userId);

        if (expense is null)
        {
            return null;
        }

        return new ExpenseFormViewModel
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Category = expense.Category,
            Date = expense.Date,
            Note = expense.Note
        };
    }

    public Task CreateExpenseAsync(ExpenseFormViewModel model, string userId)
    {
        var expense = new Expense
        {
            UserId = userId,
            Amount = model.Amount,
            Category = model.Category,
            Date = model.Date,
            Note = model.Note?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        return _expenseRepository.AddAsync(expense);
    }

    public async Task<bool> UpdateExpenseAsync(ExpenseFormViewModel model, string userId)
    {
        if (model.Id is null)
        {
            return false;
        }

        var expense = await _expenseRepository.GetByIdAsync(model.Id.Value, userId);

        if (expense is null)
        {
            return false;
        }

        expense.Amount = model.Amount;
        expense.Category = model.Category;
        expense.Date = model.Date;
        expense.Note = model.Note?.Trim();
        expense.UpdatedAt = DateTime.UtcNow;

        await _expenseRepository.UpdateAsync(expense);
        return true;
    }

    public Task<bool> DeleteExpenseAsync(int id, string userId)
    {
        return _expenseRepository.DeleteAsync(id, userId);
    }

    public async Task<bool> IsValidCategoryAsync(string category, string userId)
    {
        var categories = await _categoryRepository.GetAllForUserAsync(userId);

        return categories.Any(c => string.Equals(c.Name, category, StringComparison.OrdinalIgnoreCase));
    }
}
