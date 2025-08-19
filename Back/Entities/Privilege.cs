using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Database.Entities
{
    public class Privilege
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; }
    }
}