using FinanzApp.Web.Data;
using FinanzApp.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzApp.Web.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly ApplicationDbContext _context;

    public IncomeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Income>> GetAllByUserIdAsync(string userId)
    {
        return _context.Incomes
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.Date)
            .ThenByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public Task<Income?> GetByIdAsync(int id, string userId)
    {
        return _context.Incomes
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
    }

    public async Task AddAsync(Income income)
    {
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var affectedRows = await _context.Incomes
            .Where(i => i.Id == id && i.UserId == userId)
            .ExecuteDeleteAsync();

        return affectedRows > 0;
    }

    public Task<List<Income>> GetMonthlyIncomesAsync(string userId, int year, int month)
    {
        return _context.Incomes
            .AsNoTracking()
            .Where(i => i.UserId == userId
                && i.Date.Year == year
                && i.Date.Month == month)
            .OrderBy(i => i.Date)
            .ToListAsync();
    }

    public Task<List<Income>> GetIncomesSinceAsync(string userId, DateTime fromDate)
    {
        return _context.Incomes
            .AsNoTracking()
            .Where(i => i.UserId == userId && i.Date >= fromDate)
            .OrderBy(i => i.Date)
            .ToListAsync();
    }
}
