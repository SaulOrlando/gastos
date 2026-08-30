using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public class SavingsGoalService : ISavingsGoalService
{
    private readonly ISavingsGoalRepository _savingsGoalRepository;

    public SavingsGoalService(ISavingsGoalRepository savingsGoalRepository)
    {
        _savingsGoalRepository = savingsGoalRepository;
    }

    public async Task<List<SavingsGoalItemViewModel>> GetActiveGoalsAsync(string userId, string currencySymbol)
    {
        var goals = await _savingsGoalRepository.GetAllByUserIdAsync(userId);

        return goals
            .Where(g => !g.IsCompleted)
            .Select(g => MapToItem(g, currencySymbol))
            .OrderBy(g => g.ProgressPercentage)
            .ToList();
    }

    public async Task<bool> CreateGoalAsync(CreateGoalViewModel model, string userId)
    {
        var goal = new SavingsGoal
        {
            UserId = userId,
            Name = model.Name?.Trim() ?? string.Empty,
            TargetAmount = model.TargetAmount,
            Deadline = model.Deadline,
            CategoryTag = model.CategoryTag,
            CreatedAt = DateTime.UtcNow
        };

        await _savingsGoalRepository.AddAsync(goal);
        return true;
    }

    public async Task<bool> AddFundsAsync(AddFundsViewModel model, string userId)
    {
        if (model.GoalId <= 0)
        {
            return false;
        }

        var goal = await _savingsGoalRepository.GetByIdAsync(model.GoalId, userId);

        if (goal is null)
        {
            return false;
        }

        var entry = new SavingsEntry
        {
            GoalId = goal.Id,
            Amount = model.Amount,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _savingsGoalRepository.AddEntryAsync(entry);

        var currentAmount = goal.SavingsEntries.Sum(e => e.Amount) + model.Amount;
        if (currentAmount >= goal.TargetAmount)
        {
            goal.IsCompleted = true;
            await _savingsGoalRepository.UpdateAsync(goal);
        }

        return true;
    }

    private static SavingsGoalItemViewModel MapToItem(SavingsGoal goal, string currencySymbol)
    {
        var currentAmount = goal.SavingsEntries.Sum(e => e.Amount);
        var progress = goal.TargetAmount > 0
            ? (int)Math.Min(100, Math.Round((currentAmount / goal.TargetAmount) * 100))
            : 0;

        return new SavingsGoalItemViewModel
        {
            Id = goal.Id,
            Name = goal.Name,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = currentAmount,
            ProgressPercentage = progress,
            CategoryTag = goal.CategoryTag,
            CurrencySymbol = currencySymbol
        };
    }
}
