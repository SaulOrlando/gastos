using FinanzApp.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanzApp.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<Income> Incomes { get; set; }
    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();
    public DbSet<SavingsEntry> SavingsEntries => Set<SavingsEntry>();
    public DbSet<AITip> AITips => Set<AITip>();
    public DbSet<UserBadge> UserBadges { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(x => x.MonthlyBudget).HasColumnType("decimal(18,2)");
            e.Property(x => x.SalaryAmount).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Expense>(e =>
        {
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Category).HasMaxLength(50);
            e.HasIndex(x => new { x.UserId, x.Date }).HasDatabaseName("IX_Expense_User_Date");
            e.HasIndex(x => new { x.UserId, x.Category }).HasDatabaseName("IX_Expense_User_Category");
            e.HasOne(x => x.User).WithMany(u => u.Expenses).HasForeignKey(x => x.UserId);
        });

        builder.Entity<ExpenseCategory>(e =>
        {
            e.HasIndex(x => new { x.UserId, x.Name }).HasDatabaseName("IX_Category_User_Name");
            e.HasOne(x => x.User).WithMany(u => u.Categories).HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Income>(e =>
        {
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.HasIndex(x => new { x.UserId, x.Date }).HasDatabaseName("IX_Income_User_Date");
            e.HasIndex(x => new { x.UserId, x.Category }).HasDatabaseName("IX_Income_User_Category");
            e.HasOne(x => x.User).WithMany(u => u.Incomes).HasForeignKey(x => x.UserId);
        });

        builder.Entity<UserBadge>(e =>
        {
            e.HasIndex(x => x.UserId).HasDatabaseName("IX_UserBadge_User");
            e.HasOne(x => x.User).WithMany(u => u.UserBadges).HasForeignKey(x => x.UserId);
        });

        builder.Entity<SavingsGoal>(e =>
        {
            e.Property(x => x.TargetAmount).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.User).WithMany(u => u.SavingsGoals).HasForeignKey(x => x.UserId);
        });

        builder.Entity<SavingsEntry>(e =>
        {
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.HasIndex(x => new { x.GoalId, x.Date }).HasDatabaseName("IX_SavingsEntry_Goal");
            e.HasOne(x => x.Goal).WithMany(g => g.SavingsEntries).HasForeignKey(x => x.GoalId);
        });

        builder.Entity<AITip>(e =>
        {
            e.HasIndex(x => new { x.UserId, x.GeneratedAt }).HasDatabaseName("IX_AITip_User");
            e.HasOne(x => x.User).WithMany(u => u.AITips).HasForeignKey(x => x.UserId);
        });
    }
}
