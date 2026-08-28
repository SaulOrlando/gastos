using FinanzApp.Web.Data;
using FinanzApp.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzApp.Web.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<ExpenseCategory>> GetAllForUserAsync(string userId)
    {
        return _context.ExpenseCategories
            .AsNoTracking()
            .Where(c => c.UserId == null || c.UserId == userId)
            .OrderByDescending(c => c.UserId == null)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public Task<ExpenseCategory?> GetOwnedByIdAsync(int id, string userId)
    {
        return _context.ExpenseCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task AddAsync(ExpenseCategory category)
    {
        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ExpenseCategory category)
    {
        _context.ExpenseCategories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var deleted = await _context.ExpenseCategories
            .Where(c => c.Id == id && c.UserId == userId)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }

    public Task<bool> NameExistsAsync(string name, string userId, int? excludeId = null)
    {
        var normalized = name.Trim();
        var query = _context.ExpenseCategories
            .AsNoTracking()
            .Where(c => c.Name.ToLower() == normalized.ToLower()
                && (c.UserId == null || c.UserId == userId));

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return query.AnyAsync();
    }
}