using System.ComponentModel.DataAnnotations;
using FinanzApp.Web.Models;

namespace FinanzApp.Web.ViewModels;

public class ExpenseFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Escribe cuánto gastaste.")]
    [Range(0.01, 9_999_999, ErrorMessage = "El monto debe ser mayor a 0.")]
    [Display(Name = "Monto")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Elige una categoría para tu gasto.")]
    [EnumDataType(typeof(ExpenseCategory), ErrorMessage = "Esa categoría no existe.")]
    [Display(Name = "Categoría")]
    public ExpenseCategory Category { get; set; }

    [Required(ErrorMessage = "Dinos en qué fecha fue el gasto.")]
    [Display(Name = "Fecha")]
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;

    [StringLength(500, ErrorMessage = "La nota no puede pasar de 500 caracteres.")]
    [Display(Name = "Nota (opcional)")]
    public string? Note { get; set; }
}
