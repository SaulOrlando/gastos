using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gastos.Models;

public class AITip
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(128)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public bool? IsUseful { get; set; }

    public DateTime? RatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
}
