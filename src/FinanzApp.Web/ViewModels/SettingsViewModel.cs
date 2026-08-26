using System.ComponentModel.DataAnnotations;

namespace FinanzApp.Web.ViewModels
{
    public class SettingsViewModel
    {
        // --- Perfil de Usuario ---
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        // --- Cambio de Contraseña ---
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
        [Display(Name = "Nueva Contraseña")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string? ConfirmPassword { get; set; }

        // --- Preferencias Financieras ---
        [Required]
        [Display(Name = "Moneda Principal")]
        public string Currency { get; set; } = "USD";

        [Display(Name = "Límite de Presupuesto Mensual")]
        [Range(0, double.MaxValue, ErrorMessage = "Monto no válido")]
        public decimal? MonthlyBudgetLimit { get; set; }

        [Display(Name = "Recibir Alertas de Gastos")]
        public bool EnableNotifications { get; set; } = true;
    }
}