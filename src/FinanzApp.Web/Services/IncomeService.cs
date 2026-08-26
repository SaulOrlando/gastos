using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _incomeRepository;

    public IncomeService(IIncomeRepository incomeRepository)
    {
        _incomeRepository = incomeRepository;
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
}
