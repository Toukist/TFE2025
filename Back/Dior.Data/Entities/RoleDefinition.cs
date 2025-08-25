using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dior.Library.Entities
{
    [Table("ROLE_DEFINITION")]
    [Index(nameof(Name), IsUnique = true)]
    public class RoleDefinition
    {
        public RoleDefinition()
        {
            UserRoles = new HashSet<UserRole>();
            RoleDefinitionPrivileges = new HashSet<RoleDefinitionPrivilege>();
        }

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public long? ParentRoleId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? LastEditAt { get; set; }

        [MaxLength(100)]
        public string? LastEditBy { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; }
    }
}