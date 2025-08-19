using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Database.Entities
{
    public class RoleDefinitionPrivilege
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoleDefinitionId { get; set; }
        
        [ForeignKey(nameof(RoleDefinitionId))]
        public virtual RoleDefinition RoleDefinition { get; set; }

        [Required]
        public int PrivilegeId { get; set; }
        
        [ForeignKey(nameof(PrivilegeId))]
        public virtual Privilege Privilege { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
    }
}