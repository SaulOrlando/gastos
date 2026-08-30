using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly ICategoryRepository _categoryRepository;

    public IncomeService(
        IIncomeRepository incomeRepository,
        ICategoryRepository categoryRepository)
    {
        _incomeRepository = incomeRepository;
        _categoryRepository = categoryRepository;
    }

    public Task CreateIncomeAsync(IncomeFormViewModel model, string userId)
    {
        var income = new Income
        {
            UserId = userId,
            Amount = model.Amount,
            Category = model.Category,
            Date = model.Date,
            Note = model.Note?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        return _incomeRepository.AddAsync(income);
    }

    public async Task<IncomeFormViewModel?> GetIncomeForEditAsync(int id, string userId)
    {
        var income = await _incomeRepository.GetByIdAsync(id, userId);

        if (income is null)
        {
            return null;
        }

        return new IncomeFormViewModel
        {
            Id = income.Id,
            Amount = income.Amount,
            Category = income.Category,
            Date = income.Date,
            Note = income.Note
        };
    }

    public async Task<bool> UpdateIncomeAsync(IncomeFormViewModel model, string userId)
    {
        if (model.Id is null)
        {
            return false;
        }

        var income = await _incomeRepository.GetByIdAsync(model.Id.Value, userId);

        if (income is null)
        {
            return false;
        }

        income.Amount = model.Amount;
        income.Category = model.Category;
        income.Date = model.Date;
        income.Note = model.Note?.Trim();
        income.UpdatedAt = DateTime.UtcNow;

        await _incomeRepository.UpdateAsync(income);
        return true;
    }

    public Task<bool> DeleteIncomeAsync(int id, string userId)
    {
        return _incomeRepository.DeleteAsync(id, userId);
    }

    public async Task<bool> IsValidCategoryAsync(string category, string userId)
    {
        var categories = await _categoryRepository.GetAllForUserAsync(userId);

        return categories.Any(c => string.Equals(c.Name, category, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<IncomeListItemViewModel>> GetAllIncomesAsync(string userId)
    {
        var incomes = await _incomeRepository.GetAllByUserIdAsync(userId);

        return incomes.Select(i => new IncomeListItemViewModel
        {
            Id = i.Id,
            Amount = i.Amount,
            Category = i.Category,
            Date = i.Date,
            Note = i.Note
        }).ToList();
    }

    public async Task<List<IncomeListItemViewModel>> GetIncomesForMonthAsync(string userId, int year, int month, int? limit = null)
    {
        var incomes = await _incomeRepository.GetMonthlyIncomesAsync(userId, year, month);

        if (limit.HasValue)
        {
            incomes = incomes.OrderByDescending(i => i.Date).Take(limit.Value).ToList();
        }

        return incomes.Select(i => new IncomeListItemViewModel
        {
            Id = i.Id,
            Amount = i.Amount,
            Category = i.Category,
            Date = i.Date,
            Note = i.Note
        }).ToList();
    }
}