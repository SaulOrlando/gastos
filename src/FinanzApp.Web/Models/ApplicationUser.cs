using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FinanzApp.Web.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "MXN";

    [MaxLength]
    public string? ProfilePicture { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SalaryAmount { get; set; }

    [StringLength(50)]
    public string DepositFrequency { get; set; } = "Quincenal";

    public DateTime? DepositStartDate { get; set; }

    [Range(1, 31)]
    public int? DepositIntervalDays { get; set; }

    public DateTime? LastRecurringIncomeAt { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MonthlyBudget { get; set; }

    [Range(1, 12)]
    public int? BudgetMonth { get; set; }

    [Range(2024, 2100)]
    public int? BudgetYear { get; set; }

    public bool DarkModeEnabled { get; set; } = false;

    public bool RemindersEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public ICollection<ExpenseCategory> Categories { get; set; } = new List<ExpenseCategory>();

    public ICollection<Income> Incomes { get; set; } = new List<Income>();

    public ICollection<SavingsGoal> SavingsGoals { get; set; } = new List<SavingsGoal>();

    public ICollection<AITip> AITips { get; set; } = new List<AITip>();

    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}
