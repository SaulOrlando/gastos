using System.ComponentModel.DataAnnotations;

namespace FinanzApp.Web.ViewModels;

public class SavingsGoalListViewModel
{
    public List<SavingsGoalItemViewModel> Goals { get; set; } = new();

    public string CurrencySymbol { get; set; } = "$";
}

public class SavingsGoalItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public int ProgressPercentage { get; set; }

    public string CategoryTag { get; set; } = "General";

    public string CurrencySymbol { get; set; } = "$";
}

public class CreateGoalViewModel
{
    [Required(ErrorMessage = "Ponle un nombre a tu meta.")]
    [StringLength(100, ErrorMessage = "El nombre no puede pasar de 100 caracteres.")]
    [Display(Name = "Nombre de la meta")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indica el monto objetivo.")]
    [Range(0.01, 99_999_999, ErrorMessage = "El monto debe ser mayor a 0.")]
    [Display(Name = "Monto objetivo")]
    public decimal TargetAmount { get; set; }

    [Required(ErrorMessage = "Elige una fecha límite.")]
    [Display(Name = "Fecha límite")]
    public DateTime Deadline { get; set; } = DateTime.UtcNow.Date.AddMonths(1);

    [StringLength(50)]
    [Display(Name = "Categoría")]
    public string CategoryTag { get; set; } = "General";

    public string CurrencySymbol { get; set; } = "$";
}

public class AddFundsViewModel
{
    [Required]
    public int GoalId { get; set; }

    [Required(ErrorMessage = "Escribe cuánto quieres abonar.")]
    [Range(0.01, 99_999_999, ErrorMessage = "El monto debe ser mayor a 0.")]
    [Display(Name = "Monto a abonar")]
    public decimal Amount { get; set; }

    public string CurrencySymbol { get; set; } = "$";
}
