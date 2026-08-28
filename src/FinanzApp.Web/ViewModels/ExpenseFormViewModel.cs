using System.ComponentModel.DataAnnotations;

namespace FinanzApp.Web.ViewModels;

public class ExpenseFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Escribe cuánto gastaste.")]
    [Range(0.01, 9_999_999, ErrorMessage = "El monto debe ser mayor a 0.")]
    [Display(Name = "Monto")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Elige una categoría para tu gasto.")]
    [StringLength(50, ErrorMessage = "El nombre de la categoría es demasiado largo.")]
    [Display(Name = "Categoría")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dinos en qué fecha fue el gasto.")]
    [Display(Name = "Fecha")]
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;

    [StringLength(500, ErrorMessage = "La nota no puede pasar de 500 caracteres.")]
    [Display(Name = "Nota (opcional)")]
    public string? Note { get; set; }

    public List<CategoryViewModel> Categories { get; set; } = new();

    public string CurrencySymbol { get; set; } = "$";

    public string CurrentMonthName { get; set; } = string.Empty;

    public List<ExpenseListItemViewModel> CurrentMonthExpenses { get; set; } = new();
}