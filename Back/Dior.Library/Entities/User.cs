using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.Entities
{
    public partial class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
            UserAccessCompetencies = new HashSet<UserAccessCompetency>();
            UserAccesses = new HashSet<UserAccess>();
            AuditLogs = new HashSet<AuditLog>();
        }

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

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

        public int? TeamId { get; set; }
        public virtual Team Team { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserAccessCompetency> UserAccessCompetencies { get; set; }
        public virtual ICollection<UserAccess> UserAccesses { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
    }
}