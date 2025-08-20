using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Index(nameof(Name), IsUnique = true)] // Moved Index attribute to the class level
    public class RoleDefinition
    {
        public RoleDefinition()
        {
            ChildRoles = new List<RoleDefinition>();
            UserRoles = new List<UserRole>();
            RoleDefinitionPrivileges = new List<RoleDefinitionPrivilege>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        // Self-referencing parent
        public int? ParentRoleId { get; set; }
        [ForeignKey(nameof(ParentRoleId))]
        public RoleDefinition ParentRole { get; set; } = null!;
        public ICollection<RoleDefinition> ChildRoles { get; set; } = null!;

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

        // Navigation
        public ICollection<UserRole> UserRoles { get; set; } = null!;
        public ICollection<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; } = null!;
    }
}