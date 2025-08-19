using Dior.Library.Entities;
using System.ComponentModel.DataAnnotations;

public class User
{
    public User()
    {
        UserRoles = new List<UserRole>();
        UserAccessCompetencies = new List<UserAccessCompetency>();
        UserAccesses = new List<UserAccess>();
        AuditLogs = new List<AuditLog>();
    }

    [Key]
    public int Id { get; set; } // Un seul identifiant

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    public bool IsAdmin { get; set; }

    // Indicates whether the account is active.  Renamed from
    // IsActivate to IsActive for consistency across the codebase.
    [Required]
    public bool IsActive { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [MaxLength(100)]
    public string CreatedBy { get; set; }

    public DateTime? LastEditAt { get; set; }
    [MaxLength(100)]
    public string LastEditBy { get; set; }

    [MaxLength(255)]
    public string Email { get; set; }

    [MaxLength(50)]
    public string Phone { get; set; }
    // Clé étrangère vers la Team (nullable)
    public int? TeamId { get; set; }

    // Navigation vers la Team
    public Team? Team { get; set; } // Facultatif mais utile si tu veux accéder aux infos déquipe

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = null!;
    public ICollection<UserAccessCompetency> UserAccessCompetencies { get; set; } = null!;
    public ICollection<UserAccess> UserAccesses { get; set; } = null!;
    public ICollection<AuditLog> AuditLogs { get; set; } = null!;

    // Nom complet (optionnel, pour compatibilité)
    public string Name { get; set; }
}