using FinanzApp.Web.Data;
using FinanzApp.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzApp.Web.Repositories;

public class SavingsGoalRepository : ISavingsGoalRepository
{
    private readonly ApplicationDbContext _context;

    public SavingsGoalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<SavingsGoal>> GetAllByUserIdAsync(string userId)
    {
        return _context.SavingsGoals
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .Include(g => g.SavingsEntries)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public Task<SavingsGoal?> GetByIdAsync(int id, string userId)
    {
        return _context.SavingsGoals
            .Include(g => g.SavingsEntries)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
    }

    public async Task AddAsync(SavingsGoal goal)
    {
        _context.SavingsGoals.Add(goal);
        await _context.SaveChangesAsync();
    }

    public async Task AddEntryAsync(SavingsEntry entry)
    {
        _context.SavingsEntries.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SavingsGoal goal)
    {
        _context.SavingsGoals.Update(goal);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var affectedRows = await _context.SavingsGoals
            .Where(g => g.Id == id && g.UserId == userId)
            .ExecuteDeleteAsync();

        return affectedRows > 0;
    }
}
