using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FinanzApp.Web.ViewModels
{
    public class SettingsViewModel
    {
        // --- Perfil de Usuario ---
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Foto de Perfil")]
        public string? ProfilePicture { get; set; }

        public IFormFile? ProfilePictureFile { get; set; }

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

        // --- Configuración de Ingresos ---
        [Display(Name = "Monto del Sueldo")]
        [Range(0, double.MaxValue, ErrorMessage = "Monto no válido")]
        public decimal? SalaryAmount { get; set; }

        [Display(Name = "Frecuencia de Depósito")]
        [StringLength(50)]
        public string DepositFrequency { get; set; } = "Quincenal";

        [Display(Name = "Primer día de depósito")]
        [DataType(DataType.Date)]
        public DateTime? DepositStartDate { get; set; }

        [Display(Name = "Días entre depósitos")]
        [Range(1, 31, ErrorMessage = "Debe ser entre 1 y 31 días")]
        public int? DepositIntervalDays { get; set; }

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