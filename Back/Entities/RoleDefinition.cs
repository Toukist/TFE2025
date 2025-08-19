using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Database.Entities
{
    public class RoleDefinition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentRoleId { get; set; }
        
        [ForeignKey(nameof(ParentRoleId))]
        public virtual RoleDefinition ParentRole { get; set; }
        
        public virtual ICollection<RoleDefinition> ChildRoles { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; }
    }
}