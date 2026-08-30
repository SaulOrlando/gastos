using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanzApp.Web.Models;

public class ExpenseCategory
{
    [Key]
    public int Id { get; set; }

    public string? UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string Icon { get; set; } = "bi-tag";

    [StringLength(7)]
    public string Color { get; set; } = "#21C3D6";

    public bool IsSystem { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }
}