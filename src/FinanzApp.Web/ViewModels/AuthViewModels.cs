using System.ComponentModel.DataAnnotations;

namespace FinanzApp.Web.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Escribe tu correo electrónico.")]
    [EmailAddress(ErrorMessage = "Ese correo no parece válido, revísalo.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe tu nueva contraseña.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Tu contraseña necesita al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class VerifyOtpViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe el código de 6 dígitos.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener 6 dígitos.")]
    [RegularExpression("^[0-9]{6}$", ErrorMessage = "El código debe ser numérico de 6 dígitos.")]
    [Display(Name = "Código de verificación")]
    public string Code { get; set; } = string.Empty;
}
