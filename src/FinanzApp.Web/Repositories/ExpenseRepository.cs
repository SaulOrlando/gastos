using FinanzApp.Web.Data;
using FinanzApp.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzApp.Web.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Expense>> GetMonthlyExpensesAsync(string userId, int year, int month)
    {
        return _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId
                && e.Date.Year == year
                && e.Date.Month == month)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<Dictionary<ExpenseCategory, decimal>> GetTotalExpensesByCategoryAsync(
        string userId, int year, int month)
    {
        return await _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId
                && e.Date.Year == year
                && e.Date.Month == month)
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.Category, x => x.Total);
    }

    public Task<List<Expense>> GetExpensesSinceAsync(string userId, DateTime fromDate)
    {
        return _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.Date >= fromDate)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public Task<List<Expense>> GetAllByUserIdAsync(string userId)
    {
        return _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public Task<Expense?> GetByIdAsync(int id, string userId)
    {
        return _context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
    }

    public async Task AddAsync(Expense expense)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var affectedRows = await _context.Expenses
            .Where(e => e.Id == id && e.UserId == userId)
            .ExecuteDeleteAsync();

        return affectedRows > 0;
    }

    public async Task<List<(int Year, int Month)>> GetDistinctMonthsAsync(string userId)
    {
        return await _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(e => new { e.Date.Year, e.Date.Month })
            .Distinct()
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .Select(x => new ValueTuple<int, int>(x.Year, x.Month))
            .ToListAsync();
    }
}
