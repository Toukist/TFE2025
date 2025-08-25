using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("ROLE_DEFINITION_PRIVILEGE")]
    public class RoleDefinitionPrivilege
    {
        [Key]
        public long Id { get; set; }

        public long RoleDefinitionId { get; set; }
        public long PrivilegeId { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? LastEditAt { get; set; }

        [MaxLength(100)]
        public string? LastEditBy { get; set; }

        // Navigation properties
        public virtual RoleDefinition? RoleDefinition { get; set; }
        public virtual Privilege? Privilege { get; set; }
    }
}