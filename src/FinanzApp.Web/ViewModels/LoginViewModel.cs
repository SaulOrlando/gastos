using System.ComponentModel.DataAnnotations;

namespace FinanzApp.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Escribe tu correo electrónico.")]
    [EmailAddress(ErrorMessage = "Ese correo no parece válido, revísalo.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe tu contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recuérdame")]
    public bool RememberMe { get; set; }
}
