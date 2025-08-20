using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("USER")]
    public class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
            UserAccessCompetencies = new HashSet<UserAccessCompetency>();
            UserAccesses = new HashSet<UserAccess>();
        }

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        public string UserName { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(50)]
        [Phone]
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        // Soft delete flag (true = active, false = logically deleted)
        public bool IsActive { get; set; }

        // Soft delete metadata
        public DateTime? DeletedAt { get; set; }

        [MaxLength(100)]
        public string? DeletedBy { get; set; }

        public int? TeamId { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? LastEditAt { get; set; }

        [MaxLength(100)]
        public string? LastEditBy { get; set; }

        // Security stamp for token/session invalidation
        [MaxLength(100)]
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        // Computed convenience property (not mapped to DB)
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public virtual Team? Team { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserAccessCompetency> UserAccessCompetencies { get; set; }
        public virtual ICollection<UserAccess> UserAccesses { get; set; }
    }
}