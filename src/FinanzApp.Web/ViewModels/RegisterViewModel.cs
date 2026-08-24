using System.ComponentModel.DataAnnotations;

namespace FinanzApp.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Cuéntanos cómo te llamas.")]
    [StringLength(100, ErrorMessage = "Tu nombre no puede pasar de 100 caracteres.")]
    [Display(Name = "Nombre completo")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Necesitamos tu correo para crear tu cuenta.")]
    [EmailAddress(ErrorMessage = "Ese correo no parece válido, revísalo.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Elige una contraseña.")]
    [MinLength(6, ErrorMessage = "Tu contraseña necesita al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma tu contraseña.")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona tu moneda.")]
    [Display(Name = "Moneda")]
    public string Currency { get; set; } = "USD";
}
