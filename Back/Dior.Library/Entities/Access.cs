using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities;

[Table("ACCESS")]
public class Access
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? BadgePhysicalNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? LastEditAt { get; set; }

    [MaxLength(100)]
    public string? LastEditBy { get; set; }

    // Navigation Properties
    public virtual ICollection<UserAccess> UserAccesses { get; set; } = new List<UserAccess>();
}