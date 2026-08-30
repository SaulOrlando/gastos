using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.Services;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRecurringIncomeService _recurringIncomeService;
        private readonly ISavingsGoalRepository _savingsGoalRepository;

        public SettingsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IRecurringIncomeService recurringIncomeService,
            ISavingsGoalRepository savingsGoalRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _recurringIncomeService = recurringIncomeService;
            _savingsGoalRepository = savingsGoalRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new SettingsViewModel
            {
                FullName = string.IsNullOrWhiteSpace(user.FullName)
                    ? $"{user.FirstName} {user.LastName}".Trim()
                    : user.FullName,
                Email = user.Email ?? string.Empty,
                ProfilePicture = user.ProfilePicture,
                SalaryAmount = user.SalaryAmount,
                DepositFrequency = string.IsNullOrWhiteSpace(user.DepositFrequency) ? "Quincenal" : user.DepositFrequency,
                DepositStartDate = user.DepositStartDate,
                DepositIntervalDays = user.DepositIntervalDays,
                Currency = user.Currency,
                MonthlyBudgetLimit = user.MonthlyBudget,
                EnableNotifications = user.RemindersEnabled,
            };

            model.GoalDeductions = await LoadGoalDeductionsAsync(user.Id, model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Ajustes que vienen desde la vista como campos ocultos / no editables.
            model.Currency = user.Currency;
            model.MonthlyBudgetLimit = user.MonthlyBudget;
            model.EnableNotifications = user.RemindersEnabled;
            model.Email = user.Email ?? string.Empty;

            // Estos campos no se envían desde el formulario; se rellenan arriba desde
            // el usuario. Hay que quitar sus errores de ModelState, si los hay, para
            // que no invaliden el guardado (p. ej. Currency es [Required]).
            ModelState.Remove(nameof(SettingsViewModel.Currency));
            ModelState.Remove(nameof(SettingsViewModel.Email));
            ModelState.Remove(nameof(SettingsViewModel.MonthlyBudgetLimit));
            ModelState.Remove(nameof(SettingsViewModel.EnableNotifications));

            // DepositFrequency es un string no anulable; si el navegador no lo envía
            // (hidden vacío por usuarios previos a la migración) dispararía un error
            // [Required] implícito que impediría guardar. Se asigna un default.
            if (string.IsNullOrWhiteSpace(model.DepositFrequency))
            {
                ModelState.Remove(nameof(SettingsViewModel.DepositFrequency));
                model.DepositFrequency = "Quincenal";
            }

            if (!ModelState.IsValid)
            {
                model.GoalDeductions = await LoadGoalDeductionsAsync(user.Id, model);
                return View("Index", model);
            }

            var nameParts = model.FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            // Capturamos los valores previos ANTES de sobrescribirlos, para poder
            // detectar si el calendario de depósitos cambió (comparar después sería
            // siempre falso porque user ya tendría los valores del modelo).
            var previousSchedule = (user.SalaryAmount, user.DepositFrequency, user.DepositStartDate, user.DepositIntervalDays);

            user.FullName = model.FullName?.Trim() ?? string.Empty;
            user.FirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            user.LastName = nameParts.Length > 1 ? string.Join(' ', nameParts.Skip(1)) : string.Empty;
            user.SalaryAmount = model.SalaryAmount;
            user.DepositFrequency = string.IsNullOrWhiteSpace(model.DepositFrequency) ? "Quincenal" : model.DepositFrequency.Trim();
            user.DepositStartDate = model.DepositStartDate;
            user.DepositIntervalDays = model.DepositIntervalDays;

            if (user.DepositFrequency == "Personalizado"
                && (user.DepositStartDate is null || user.DepositIntervalDays is null))
            {
                ModelState.AddModelError(string.Empty, "Para el depósito personalizado indica el primer día y la cantidad de días.");
                model.GoalDeductions = await LoadGoalDeductionsAsync(user.Id, model);
                return View("Index", model);
            }

            // Si cambió la configuración del depósito, se reinicia el progreso para
            // recalcular los próximos depósitos automáticos de forma correcta.
            var scheduleChanged = previousSchedule.SalaryAmount != user.SalaryAmount
                || previousSchedule.DepositFrequency != user.DepositFrequency
                || previousSchedule.DepositStartDate != user.DepositStartDate
                || previousSchedule.DepositIntervalDays != user.DepositIntervalDays;
            if (scheduleChanged)
            {
                user.LastRecurringIncomeAt = null;
            }

            if (model.ProfilePictureFile is not null && model.ProfilePictureFile.Length > 0)
            {
                var newPicture = await SaveProfilePictureAsync(model.ProfilePictureFile);
                if (newPicture != null)
                {
                    user.ProfilePicture = newPicture;
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _recurringIncomeService.ProcessAsync(user);

                // Regenera la cookie de autenticación (nombre) y la marca de seguridad.
                // No se incluye la foto en los claims para evitar inflar la cookie.
                await _userManager.UpdateSecurityStampAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Cambios guardados correctamente.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Index", model);
        }

        private static async Task<string?> SaveProfilePictureAsync(IFormFile file)
        {
            if (file.Length <= 0) return null;

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) return null;

            // Límite de tamaño: 1.5 MB máx para no inflar la base de datos
            const int maxBytes = 1_500_000;
            if (file.Length > maxBytes) return null;

            var mime = ext switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return $"data:{mime};base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        private async Task<List<GoalDeductionViewModel>> LoadGoalDeductionsAsync(string userId, SettingsViewModel model)
        {
            var goals = await _savingsGoalRepository.GetAllByUserIdAsync(userId);

            return goals
                .Where(g => !g.IsCompleted)
                .OrderBy(g => g.CreatedAt)
                .Select(g => new GoalDeductionViewModel
                {
                    GoalId = g.Id,
                    Name = g.Name,
                    CategoryTag = string.IsNullOrWhiteSpace(g.CategoryTag) ? "General" : g.CategoryTag,
                    MonthlyContribution = g.MonthlyContribution
                })
                .ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "Debes completar los campos de contraseña.");
                model.GoalDeductions = await LoadGoalDeductionsAsync(user.Id, model);
                return View("Index", model);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Contraseña cambiada con éxito.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.GoalDeductions = await LoadGoalDeductionsAsync(user.Id, model);
            return View("Index", model);
        }
    }
}