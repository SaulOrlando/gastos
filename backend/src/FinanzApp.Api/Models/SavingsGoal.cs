using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gastos.Models;

public class SavingsGoal
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(128)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto objetivo debe ser mayor a 0.")]
    public decimal TargetAmount { get; set; }

    public DateTime Deadline { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsCompleted { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    public ICollection<SavingsEntry> SavingsEntries { get; set; } = new List<SavingsEntry>();
}
