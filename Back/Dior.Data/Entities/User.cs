using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities;

[Table("USER")]
public class User
{
    public User()
    {
        UserRoles = new HashSet<UserRole>();
        UserAccesses = new HashSet<UserAccess>();
        UserAccessCompetencies = new HashSet<UserAccessCompetency>();
        CreatedAuditLogs = new HashSet<AuditLog>();
        CreatedNotifications = new HashSet<Notification>();
    }

    [Key]
    public int Id { get; set; } // BIGINT IDENTITY dans la DB

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty; // Correspond exactement à la DB

    public bool IsActive { get; set; } = true;

    public int? TeamId { get; set; }

    [MaxLength(50)]
    public string? BadgePhysicalNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? LastEditAt { get; set; }

    [MaxLength(100)]  
    public string? LastEditBy { get; set; }

    // Navigation properties
    [ForeignKey("TeamId")]
    public virtual Team? Team { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<UserAccess> UserAccesses { get; set; }
    public virtual ICollection<UserAccessCompetency> UserAccessCompetencies { get; set; }
    public virtual ICollection<AuditLog> CreatedAuditLogs { get; set; }
    public virtual ICollection<Notification> CreatedNotifications { get; set; }
}