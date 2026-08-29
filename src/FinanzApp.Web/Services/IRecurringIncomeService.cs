using FinanzApp.Web.Models;

namespace FinanzApp.Web.Services;

public interface IRecurringIncomeService
{
    /// <summary>
    /// Crea los ingresos automáticos (sueldo por frecuencia de depósito) que ya
    /// estén vencidos y aún no se hayan registrado, para un usuario dado.
    /// </summary>
    Task ProcessAsync(ApplicationUser user);
}
