using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities;

[Table("USER")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? BadgePhysicalNumber { get; set; }

    public int? TeamId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? LastEditAt { get; set; }

    [MaxLength(100)]
    public string? LastEditBy { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(TeamId))]
    public virtual Team? Team { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<UserAccess> UserAccesses { get; set; } = new List<UserAccess>();
    public virtual ICollection<UserAccessCompetency> UserAccessCompetencies { get; set; } = new List<UserAccessCompetency>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}