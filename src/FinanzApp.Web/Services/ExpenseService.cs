using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<List<ExpenseListItemViewModel>> GetExpensesForUserAsync(string userId)
    {
        var expenses = await _expenseRepository.GetAllByUserIdAsync(userId);

        return expenses.Select(e => new ExpenseListItemViewModel
        {
            Id = e.Id,
            Amount = e.Amount,
            Category = e.Category,
            Date = e.Date,
            Note = e.Note
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
}
